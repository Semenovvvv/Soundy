using System.Runtime.CompilerServices;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;

namespace Soundy.ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController :  ControllerBase
    {
        private readonly FileGrpcService.FileGrpcServiceClient _client;

        public FileController(FileGrpcService.FileGrpcServiceClient client)
        {
            _client = client;
        }

        [HttpGet("stream/{id}")]
        public async IAsyncEnumerable<byte[]> StreamAsync(string id, [EnumeratorCancellation] CancellationToken ct)
        {
             var request = new DownloadTrackFileRequest
            {
                TrackId = id
            };

            var response = _client.DownloadTrackFile(request, cancellationToken: ct).ResponseStream;
            var stream = response;

            while (await stream.MoveNext(ct).ConfigureAwait(false))
            {
                await Task.Delay(100, ct);
                yield return stream.Current.ToByteArray();
            }
        }

        [HttpPost("image")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            using var call = _client.UploadImage();

            await call.RequestStream.WriteAsync(new UploadImageRequest
            {
                Metadata = new ImageMetadata
                {
                    EntityType = "TRACK", // можно передавать параметром
                    EntityId = "track_123",
                    FileName = file.FileName,
                    ContentType = file.ContentType
                }
            });

            var buffer = new byte[64 * 1024];
            using var stream = file.OpenReadStream();
            int bytesRead;

            while ((bytesRead = await stream.ReadAsync(buffer)) > 0)
            {
                await call.RequestStream.WriteAsync(new UploadImageRequest
                {
                    Chunk = ByteString.CopyFrom(buffer, 0, bytesRead)
                });
            }

            await call.RequestStream.CompleteAsync();

            var response = await call.ResponseAsync;

            return Ok(new { url = response.Url });
        }
    }
}
