using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Soundy.ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController :  ControllerBase
    {
        private readonly TrackFileGrpcService.TrackFileGrpcServiceClient _client;

        public FileController(TrackFileGrpcService.TrackFileGrpcServiceClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Получение пользователя по идентификатору
        /// </summary>
        [HttpGet("stream/{id}")]
        public async Task<FileStreamResult> StreamAsync(string id)
        {
            var request = new DownloadTrackFileRequest()
            {
                TrackId = id
            };
            var response = _client.DownloadTrackFile(request);
            var stream = new MemoryStream(Encoding.ASCII.GetBytes("Hello World"));
            return new FileStreamResult(stream, MediaTypeNames.Text.RichText);
        }
    }
}
