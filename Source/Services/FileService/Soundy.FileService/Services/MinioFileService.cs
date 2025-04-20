using Amazon.S3;
using Amazon.S3.Model;
using Grpc.Core;
using Soundy.FileService.Interfaces;
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
        }

        public async Task<string> UploadTrackAsync(string trackId, Stream fileStream, CancellationToken ct = default)
        {
            var request = new PutObjectRequest()
            {
                BucketName = _bucketName,
                Key = trackId,
                InputStream = fileStream
            };

            await _s3Client.PutObjectAsync(request, ct);

            var a = await _s3Client.GetObjectAsync(_bucketName, trackId, ct);
            if (a is not null)
                return $"http://minio:9000/{_bucketName}/{trackId}";
            throw new RpcException(new Status(StatusCode.NotFound, ""));
        }

        public async Task<Stream> DownloadTrackAsync(string trackId, CancellationToken ct = default)
        {
            var response = await _s3Client.GetObjectAsync(_bucketName, trackId, ct);

            return response.ResponseStream;
        }
    }
}
