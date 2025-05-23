using System.Diagnostics;
using System.Net.Mime;
using Amazon.S3;
using Amazon.S3.Model;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc.Formatters;
using Service.File;
using Soundy.FileService.Constants;
using Soundy.FileService.Interfaces;
using Soundy.SharedLibrary.Common.Response;
using Soundy.SharedLibrary.S3;

namespace Soundy.FileService.Services
{
    public class MinioFileService : ITrackFileService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public MinioFileService(IAmazonS3 s3Client, IConfiguration configuration)
        {
            var awsOptions = configuration.GetSection(S3Options.S3).Get<S3Options>();
            _bucketName = configuration.GetSection(S3Options.S3).Get<S3Options>()?.BucketName;
            _s3Client = s3Client;
        }

        public async Task StreamTrackAsync(string trackId, string fileName, Func<byte[], string, Task> onChunk, CancellationToken ct)
        {
            var objectName = $"{trackId}/{fileName}";

            try
            {
                using var memoryStream = new MemoryStream();

                var getRequest = new GetObjectRequest()
                {
                    BucketName = Buckets.Track,
                    Key = objectName,
                };
                using var response = await _s3Client.GetObjectAsync(getRequest, ct);
                await using var stream = response.ResponseStream;

                var buffer = new byte[64 * 1024]; // 64 KB
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer, ct)) > 0)
                {
                    await onChunk(buffer[..bytesRead], response.Headers.ContentType);
                }
            }
            catch (Exception ex) when (ex is AmazonS3Exception or FileNotFoundException)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Track file not found: {objectName}"));
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error streaming track: {ex.Message}"));
            }
        }

        public async Task<UploadTrackResponse> UploadTrack(IAsyncStreamReader<UploadTrackRequest?> requestStream, CancellationToken ct = default)
        {
            UploadTrackRequest? firstRequest = null;
            var tempDir = Path.Combine(Path.GetTempPath(), "tracks", Guid.NewGuid().ToString());
            string inputPath = null;
            string trackId = null;

            try
            {
                // Создаем временную папку
                var directory = Directory.CreateDirectory(tempDir);

                // Читаем первый чанк, чтобы получить метаданные
                if (await requestStream.MoveNext(ct))
                {
                    firstRequest = requestStream.Current;
                    trackId = firstRequest.TrackId;
                    inputPath = Path.Combine(tempDir, firstRequest.FileName);

                    // Сохраняем первый чанк
                    await using var fileStream = new FileStream(inputPath, FileMode.Create, FileAccess.Write);
                    await fileStream.WriteAsync(firstRequest.Chunk.ToByteArray(), ct);

                    // Читаем остальные чанки
                    while (await requestStream.MoveNext(ct))
                    {
                        var chunk = requestStream.Current;
                        await fileStream.WriteAsync(chunk.Chunk.ToByteArray(), ct);
                    }
                }

                // Конвертируем в HLS
                await ConvertToHls(inputPath, tempDir, ct);

                directory.GetFiles(firstRequest.FileName).First().Delete();

                // Загружаем в MinIO
                await UploadHlsFilesToMinIO(trackId, tempDir, ct);

                

                // Формируем URL
                var playlistUrl = $"http://localhost:9001/{trackId}/index.m3u8";
                return new UploadTrackResponse { Url = playlistUrl };
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        public async Task<Stream> DownloadTrackAsync(string trackId, CancellationToken ct = default)
        {
            var response = await _s3Client.GetObjectAsync(_bucketName, trackId, ct);

            return response.ResponseStream;
        }

        public async Task<UploadImageResponse> UploadImage(UploadImageRequest request, CancellationToken ct = default)
        {
            var bucketName = Buckets.Image;
            var objectName = $"{Guid.NewGuid()}";

            var stream = new MemoryStream(request.Chunk.ToByteArray());

            var putRequest = new PutObjectRequest()
            {
                BucketName = bucketName,
                ContentType = request.ContentType,
                Key = objectName,
                InputStream = stream
            };

            await _s3Client.PutObjectAsync(putRequest, ct);

            var fileUrl = objectName;
            return new UploadImageResponse() { Url = fileUrl };
        }

        public async Task<DownloadImageResponse> DownloadImage(DownloadImageRequest request, CancellationToken ct = default)
        {
            var bucketName = Buckets.Image;
            var objectName = request.ImageId;

            var memoryStream = new MemoryStream();

            GetObjectResponse response;

            try
            {
                var getReq = new GetObjectRequest()
                {
                    BucketName = bucketName,
                    Key = objectName
                };

                response = await _s3Client.GetObjectAsync(getReq, ct);
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Image not found"));
            }

            await response.ResponseStream.CopyToAsync(memoryStream, ct);

            return new DownloadImageResponse
            {
                Chunk = Google.Protobuf.ByteString.CopyFrom(memoryStream.ToArray()),
                ContentType = response.Headers.ContentType
            };
        }

        private async Task ConvertToHls(string inputPath, string outputDir, CancellationToken ct = default)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-i \"{inputPath}\" -codec:a aac -b:a 128k -hls_time 4 -hls_playlist_type vod -hls_segment_filename \"{outputDir}/segment_%03d.ts\" \"{outputDir}/index.m3u8\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processStartInfo);
            await process.WaitForExitAsync(ct);

            if (process.ExitCode != 0)
            {
                var error = await process.StandardError.ReadToEndAsync(ct);
                throw new RpcException(new Status(StatusCode.Internal, $"FFmpeg failed: {error}"));
            }
        }

        private async Task UploadHlsFilesToMinIO(string trackId, string localDir, CancellationToken ct = default)
        {
            foreach (var filePath in Directory.GetFiles(localDir))
            {
                var fileName = Path.GetFileName(filePath);
                var objectName = $"{trackId}/{fileName}";

                await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var putRequest = new PutObjectRequest()
                {
                    BucketName = Buckets.Track,
                    Key = objectName,
                    InputStream = fileStream,
                    ContentType = ContentTypeMap.GetContentType(fileName)
                };

                await _s3Client.PutObjectAsync(putRequest, ct);
            }
        }

        public static class ContentTypeMap
        {
            public static string GetContentType(string fileName)
            {
                var ext = Path.GetExtension(fileName).ToLowerInvariant();
                return ext switch
                {
                    ".ts" => "video/MP2T",
                    ".m3u8" => "application/vnd.apple.mpegurl",
                    _ => "application/octet-stream"
                };
            }
        }
    }
}
