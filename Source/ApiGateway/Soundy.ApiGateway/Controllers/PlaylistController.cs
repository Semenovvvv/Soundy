using Microsoft.AspNetCore.Mvc;
using Soundy.SharedLibrary.Contracts.Playlist;

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
    public async Task<IActionResult> CreateAsync([FromBody] Soundy.SharedLibrary.Contracts.Playlist.CreateRequest dto, CancellationToken ct = default)
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
    public async Task<IActionResult> GetListByAuthorIdAsync([FromRoute] string id, CancellationToken ct = default)
    {
        var request = new GetListByAuthorIdRequest { AuthorId = id };
        var response = await _playlistService.GetListByAuthorIdAsync(request, cancellationToken: ct);
        return Ok(response);
    }

    /// <summary>
    /// Получает избранный плейлист.
    /// </summary>
    [HttpGet("favorite/{id}")]
    public async Task<IActionResult> GetFavoriteAsync([FromRoute] GetFavoriteRequest dto, CancellationToken ct = default)
    {
        var response = await _playlistService.GetFavoriteAsync(dto, cancellationToken: ct);
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