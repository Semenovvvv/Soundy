using Amazon.S3;
using Google.Protobuf;
using Grpc.Core;
using Service.File;
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

        public override async Task GetTrackStream(GetTrackRequest request, IServerStreamWriter<GetTrackResponse> responseStream, ServerCallContext context)
        {
            var onChunk = async (byte[] chunk, string contentType) =>
            {
                await responseStream.WriteAsync(new GetTrackResponse
                {
                    Chunk = Google.Protobuf.ByteString.CopyFrom(chunk),
                    ContentType = contentType
                });
            };

            await _fileService.StreamTrackAsync(request.TrackId, request.FileName, onChunk, context.CancellationToken);
        }

        public override async Task<UploadTrackResponse> UploadTrack(IAsyncStreamReader<UploadTrackRequest?> requestStream, ServerCallContext context)
        {
            var response = await _fileService.UploadTrack(requestStream, context.CancellationToken);
            return response;
        }

        public override async Task<UploadImageResponse> UploadImage(UploadImageRequest request, ServerCallContext context)
        {
            var response = await _fileService.UploadImage(request, context.CancellationToken);
            return response;
        }

        public override async Task<DownloadImageResponse> DownloadImage(DownloadImageRequest request, ServerCallContext context)
        {
            var response = await _fileService.DownloadImage(request, context.CancellationToken);
            return response;
        }
    }
}
