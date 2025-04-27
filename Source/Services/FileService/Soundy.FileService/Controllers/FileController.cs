using Amazon.S3;
using Amazon.S3.Model;
using Google.Protobuf;
using Grpc.Core;
using Soundy.FileService.Interfaces;
using Soundy.SharedLibrary.S3;

namespace Soundy.FileService.Controllers
{
    public class FileGrpcController : TrackFileGrpcService.TrackFileGrpcServiceBase
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
    }
}
