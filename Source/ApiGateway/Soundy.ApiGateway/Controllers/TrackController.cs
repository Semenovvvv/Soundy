using Microsoft.AspNetCore.Mvc;
using Service.Track;
using Soundy.ApiGateway.Configurations;

namespace Soundy.ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrackController : ControllerBase
{
    private readonly TrackGrpcService.TrackGrpcServiceClient _trackService;

    public TrackController(TrackGrpcService.TrackGrpcServiceClient trackService)
    {
        _trackService = trackService;
    }

    /// <summary>
    /// Создает новый трек.
    /// </summary>
    [HttpPost]
    [JwtAuthorize]
    public async Task<IActionResult> CreateAsync([FromBody] CreateRequest dto, CancellationToken ct = default)
    {
        var response = await _trackService.CreateAsync(dto, cancellationToken: ct);
        return Ok(response);
    }

    /// <summary>
    /// Получает трек по ID.
    /// </summary>
    [HttpGet("{id}")]
    [JwtAuthorize]
    public async Task<IActionResult> GetByIdAsync([FromRoute] string id, [FromQuery] string? userId = null, CancellationToken ct = default)
    {
        var request = new GetByIdRequest { Id = id, UserId = userId };
        var response = await _trackService.GetByIdAsync(request, cancellationToken: ct);
        if (response == null)
        {
            return NotFound($"Трек с ID {request.Id} не найден.");
        }
        return Ok(response);
    }

    /// <summary>
    /// Выполняет поиск треков.
    /// </summary>
    [HttpGet("search")]
    [JwtAuthorize]
    public async Task<IActionResult> SearchAsync([FromQuery] string pattern, [FromQuery] int pageSize = 10, [FromQuery] int pageNum = 1, [FromQuery] string? userId = null, CancellationToken ct = default)
    {
        //dto.UserId = userId;
        var dto = new SearchRequest()
        {
            PageNum = pageNum,
            PageSize = pageSize,
            Pattern = pattern
        };
        var response = await _trackService.SearchAsync(dto, cancellationToken: ct);
        return Ok(response);
    }

    /// <summary>
    /// Обновляет трек.
    /// </summary>
    [HttpPut("{id}")]
    [JwtAuthorize]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateRequest dto, CancellationToken ct = default)
    {
        var response = await _trackService.UpdateAsync(dto, cancellationToken: ct);
        return Ok(response);
    }

    /// <summary>
    /// Удаляет трек.
    /// </summary>
    [HttpDelete("{id}")]
    [JwtAuthorize]
    public async Task<IActionResult> DeleteAsync([FromRoute] string id, CancellationToken ct = default)
    {
        var request = new DeleteRequest { Id = id };
        var response = await _trackService.DeleteAsync(request, cancellationToken: ct);
        return Ok(response);
    }

    /// <summary>
    /// Лайкает трек пользователем.
    /// </summary>
    [HttpPost("like")]
    [JwtAuthorize]
    public async Task<IActionResult> LikeTrackAsync([FromBody] LikeTrackRequest request, CancellationToken ct = default)
    {
        var response = await _trackService.LikeTrackAsync(request, cancellationToken: ct);
        return Ok(response);
    }

    /// <summary>
    /// Удаляет лайк трека пользователя.
    /// </summary>
    [HttpDelete("like")]
    [JwtAuthorize]
    public async Task<IActionResult> UnlikeTrackAsync([FromBody] UnlikeTrackRequest request, CancellationToken ct = default)
    {
        var response = await _trackService.UnlikeTrackAsync(request, cancellationToken: ct);
        return Ok(response);
    }

    /// <summary>
    /// Получает список лайкнутых треков пользователя.
    /// </summary>
    [HttpGet("liked/{userId}")]
    [JwtAuthorize]
    public async Task<IActionResult> GetLikedTracksAsync([FromRoute] string userId, CancellationToken ct = default)
    {
        var request = new GetLikedTracksRequest { UserId = userId };
        var response = await _trackService.GetLikedTracksAsync(request, cancellationToken: ct);
        return Ok(response);
    }

    /// <summary>
    /// Получает список треков по ID автора.
    /// </summary>
    [HttpGet("author/{authorId}")]
    [JwtAuthorize]
    public async Task<IActionResult> GetByAuthorIdAsync([FromRoute] string authorId, CancellationToken ct = default)
    {
        var request = new GetListByUserIdRequest { UserId = authorId };
        var response = await _trackService.GetListByUserIdAsync(request, cancellationToken: ct);
        return Ok(response);
    }
}