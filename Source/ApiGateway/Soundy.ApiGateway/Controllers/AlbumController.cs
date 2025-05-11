using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Service.Album;

namespace Soundy.ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlbumController : ControllerBase
    {
        private readonly AlbumGrpcService.AlbumGrpcServiceClient _albumService;

        public AlbumController(AlbumGrpcService.AlbumGrpcServiceClient albumService)
        {
            _albumService = albumService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateRequest request, [EnumeratorCancellation] CancellationToken ct)
        {
            var response = await _albumService.CreateAsync(request, cancellationToken: ct);
            return Ok(response);
        }

        [HttpPost("{albumId}")]
        public async Task<IActionResult> AddTrackAsync(string albumId, [FromBody] string trackId, [EnumeratorCancellation] CancellationToken ct)
        {
            var request = new AddTrackRequest()
            {
                Id = albumId,
                TrackId = trackId
            };
            var response = await _albumService.AddTrackAsync(request, cancellationToken: ct);
            return Ok(response);
        }
    }
}
