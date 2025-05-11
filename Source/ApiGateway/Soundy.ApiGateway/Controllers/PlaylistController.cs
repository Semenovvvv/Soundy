using Microsoft.AspNetCore.Mvc;
using Service.Playlist;
using Soundy.SharedLibrary.Common.Response;

namespace Soundy.ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlaylistController : ControllerBase
{
    private readonly PlaylistGrpcService.PlaylistGrpcServiceClient _playlistService;

    public PlaylistController(PlaylistGrpcService.PlaylistGrpcServiceClient playlistService)
    {
        _playlistService = playlistService;
    }

    /// <summary>
    /// Создает новый плейлист.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateRequest dto, CancellationToken ct = default)
    {
        var response = await _playlistService.CreateAsync(dto, cancellationToken: ct);
        return Ok(response);
    }

    /// <summary>
    /// Добавляет плейлист в избранное.
    /// </summary>
    [HttpPost("favorite")]
    public async Task<IActionResult> CreateFavoriteAsync([FromBody] CreateFavoriteRequest dto, CancellationToken ct = default)
    {
        var response = await _playlistService.CreateFavoriteAsync(dto, cancellationToken: ct);
        return Ok(response);
    }

    /// <summary>
    /// Получает плейлист по ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] string id, CancellationToken ct = default)
    {
        var request = new GetByIdRequest() { Id = id };
        var response = await _playlistService.GetByIdAsync(request, cancellationToken: ct);
        return Ok(response);
    }

    /// <summary>
    /// Получает список плейлистов по ID автора.
    /// </summary>
    [HttpGet("author/{authorId}")]
    public async Task<IActionResult> GetListByAuthorIdAsync([FromRoute] string authorId, CancellationToken ct = default)
    {
        var request = new GetListByAuthorIdRequest { AuthorId = authorId };
        var response = await _playlistService.GetListByAuthorIdAsync(request, cancellationToken: ct);
        return Ok(response);
    }

    /// <summary>
    /// Получает избранный плейлист.
    /// </summary>
    [HttpGet("favorite/{id}")]
    public async Task<IActionResult> GetFavoriteAsync(string id, CancellationToken ct = default)
    {
        var dto = new GetFavoriteRequest(){ AuthorId = id};
        var response = await _playlistService.GetFavoriteAsync(dto, cancellationToken: ct);
        return Ok(response);
    }

    [HttpPost("{playlistId}/tracks")]
    public async Task<IActionResult> AddTrack(string playlistId, [FromBody] string trackId, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(trackId))
            return BadRequest("TrackId is required");

        var dto = new AddTrackRequest
        {
            PlaylistId = playlistId,
            TrackId = trackId
        };

        var response = await _playlistService.AddTrackAsync(dto, cancellationToken: ct);

        return Ok(response);
    }

    /// <summary>
    /// Обновляет плейлист.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateRequest dto, CancellationToken ct = default)
    {
        var response = await _playlistService.UpdateAsync(dto, cancellationToken: ct);
        return Ok(response);
    }

    /// <summary>
    /// Удаляет плейлист.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] string id, CancellationToken ct = default)
    {
        var request = new DeleteRequest { Id = id };
        var response = await _playlistService.DeleteAsync(request, cancellationToken: ct);
        return Ok(response);
    }
}