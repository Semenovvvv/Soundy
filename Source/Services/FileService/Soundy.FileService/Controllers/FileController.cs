using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Google.Protobuf;
using Grpc.Core;
using Soundy.FileService.Constants;
using Soundy.FileService.Interfaces;
using Soundy.SharedLibrary.S3;

namespace Soundy.FileService.Controllers
{
    public class FileGrpcController : FileGrpcService.FileGrpcServiceBase
    {
        private ITrackFileService _fileService;
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public FileGrpcController(ITrackFileService fileService, IAmazonS3 s3Client, IConfiguration configuration)
        {
            _fileService = fileService;
            _s3Client = s3Client;
            var awsOptions = configuration.GetSection(S3Options.S3).Get<S3Options>();
            _bucketName = configuration.GetSection(S3Options.S3).Get<S3Options>()?.BucketName;
        }

        public override async Task<UploadTrackFileResponse> UploadTrackFile(IAsyncStreamReader<UploadTrackFileRequest> requestStream, ServerCallContext context)
        {
            var fileName = string.Empty;
            var tempFile = Path.GetTempFileName();
            string? trackId = null;
            await using (var stream = new FileStream(tempFile, FileMode.Create))
            {
                var isLastReceived = false;

                while (await requestStream.MoveNext() && !isLastReceived)
                {
                    var request = requestStream.Current;
                    trackId ??= request.TrackId;

                    if (!string.IsNullOrEmpty(request.TrackId))
                        fileName = request.TrackId;

                    if (request.Chunk != null && request.Chunk.Length > 0)
                        await stream.WriteAsync(request.Chunk.ToByteArray(), 0, request.Chunk.Count());
                }

                var putRequest = new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    Key = fileName,
                    InputStream = stream
                };

                await _s3Client.PutObjectAsync(putRequest);

                return new UploadTrackFileResponse
                {
                    FileUrl = $"http://minio:9000/{_bucketName}/{trackId}"
                };
            }
        }

        public override async Task DownloadTrackFile(DownloadTrackFileRequest request, IServerStreamWriter<DownloadTrackFileResponse> responseStream, ServerCallContext context)
        {
            var stream = await _s3Client.GetObjectAsync(_bucketName, request.TrackId);

            byte[] buffer = new byte[8];
            int bytesRead;

            while ((bytesRead = await stream.ResponseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                if (context.CancellationToken.IsCancellationRequested)
                    break;

                await responseStream.WriteAsync(
                    new DownloadTrackFileResponse() { Chunk = ByteString.CopyFrom(buffer, 0, bytesRead) });
            }
        }

        public override async Task<UploadImageResponse> UploadImage(IAsyncStreamReader<UploadImageRequest> requestStream, ServerCallContext context)
        {
            ImageMetadata? metadata = null;
            var memoryStream = new MemoryStream();

            while (await requestStream.MoveNext())
            {
                var req = requestStream.Current;
                if (req.Metadata != null)
                {
                    metadata = req.Metadata;
                }
                else if (req.Chunk != null)
                {
                    await memoryStream.WriteAsync(req.Chunk.ToByteArray());
                }
            }

            if (metadata == null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Missing metadata"));

            await memoryStream.FlushAsync();

            var key = $"{metadata.EntityType}/{metadata.EntityId}/avatar_{Guid.NewGuid()}{Path.GetExtension(metadata.FileName)}";

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = memoryStream,
                Key = key,
                BucketName = Buckets.Image,
                ContentType = metadata.ContentType ?? "application/octet-stream"
            };

            var transferUtil = new TransferUtility(_s3Client);
            await transferUtil.UploadAsync(uploadRequest, context.CancellationToken);
            var fileUrl = $"http://minio:9000/{Buckets.Image}/{key}";

            return new UploadImageResponse { Url = fileUrl };
        }

        public override async Task<DownloadImageResponse> DownloadImage(DownloadImageRequest request, IServerStreamWriter<DownloadImageResponse> responseStream, ServerCallContext context)
        {
            //var key = $"{request.EntityType}/{request.EntityId}/avatar.jpg";
            try
            {
                var getObjectRequest = new GetObjectRequest
                {
                    BucketName = Buckets.Image,
                    //Key = key
                };

                using var response = await _s3Client.GetObjectAsync(getObjectRequest, context.CancellationToken);

                if (response == null)
                    throw new RpcException(new Status(StatusCode.NotFound, "File not found"));

                await foreach (var chunk in ReadStreamInChunks(response.ResponseStream))
                {
                    await responseStream.WriteAsync(new DownloadImageResponse
                    {
                        Chunk = ByteString.CopyFrom(chunk)
                    });
                }

                return new DownloadImageResponse();
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "File not found"));
            }
        }

        private async IAsyncEnumerable<byte[]> ReadStreamInChunks(Stream stream, int bufferSize = 64 * 1024)
        {
            var buffer = new byte[bufferSize];
            int bytesRead;

            while ((bytesRead = await stream.ReadAsync(buffer)) > 0)
            {
                yield return buffer.AsMemory(0, bytesRead).ToArray();
            }
        }
    }
}
