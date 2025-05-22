using System.Runtime.CompilerServices;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Service.File;

namespace Soundy.ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly FileGrpcService.FileGrpcServiceClient _client;

        public FileController(FileGrpcService.FileGrpcServiceClient client)
        {
            _client = client;
        }

        [HttpPost("image/upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var request = new UploadImageRequest()
            {
                Chunk = Google.Protobuf.ByteString.CopyFrom(memoryStream.ToArray()),
                ContentType = file.ContentType
            };

            var reply = _client.UploadImage(request);
            return Ok(new { url = reply.Url });
        }

        [HttpGet("image/{id}")]
        public async Task<IActionResult> GetImage(string id)
        {
            var request = new DownloadImageRequest { ImageId = id };
            var response = _client.DownloadImage(request);

            return File(response.Chunk.ToByteArray(), response.ContentType);
        }

        [HttpPost("track/upload")]
        public async Task<IActionResult> UploadTrack(IFormFile file, [FromForm] string id)
        {
            using var call = _client.UploadTrack();

            var buffer = new byte[81920];
            await using var fileStream = file.OpenReadStream();

            int read;
            while ((read = await fileStream.ReadAsync(buffer)) > 0)
            {
                await call.RequestStream.WriteAsync(new UploadTrackRequest
                {
                    TrackId = id,
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    Chunk = Google.Protobuf.ByteString.CopyFrom(buffer, 0, read)
                });
            }

            await call.RequestStream.CompleteAsync();
            var response = await call.ResponseAsync;

            return Ok(new { url = response.Url });
        }

        [HttpGet("track/{trackId}/{fileName}.m3u8")]
        public Task GetHlsPlaylist(string trackId, string fileName)
        {
            return ProxyToMinIo(trackId, $"{fileName}.m3u8");
        }

        [HttpGet("track/{trackId}/{fileName}.ts")]
        public Task GetHlsSegment(string trackId, string fileName)
        {
            return ProxyToMinIo(trackId, $"{fileName}.ts");
        }

        private async Task ProxyToMinIo(string trackId, string fileName)
        {
            var request = new GetTrackRequest
            {
                TrackId = trackId,
                FileName = fileName
            };

            using var call = _client.GetTrackStream(request);

            Response.ContentType = null;

            await foreach (var response in call.ResponseStream.ReadAllAsync())
            {
                if (string.IsNullOrEmpty(Response.ContentType))
                {
                    Response.ContentType = response.ContentType;
                }

                await Response.Body.WriteAsync(response.Chunk.ToByteArray());
                await Response.Body.FlushAsync();
            }
        }
    }
}
