﻿using Microsoft.AspNetCore.Mvc;
using Service.Track;

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
    public async Task<IActionResult> CreateAsync([FromBody] CreateRequest dto, CancellationToken ct = default)
    {
        var response = await _trackService.CreateAsync(dto, cancellationToken: ct);
        return Ok(response);
    }

    /// <summary>
    /// Получает трек по ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] string id, CancellationToken ct = default)
    {
        var request = new GetByIdRequest { Id = id };
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
    public async Task<IActionResult> SearchAsync([FromQuery] SearchRequest dto, CancellationToken ct = default)
    {
        var response = await _trackService.SearchAsync(dto, cancellationToken: ct);
        return Ok(response);
    }

    /// <summary>
    /// Обновляет трек.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateRequest dto, CancellationToken ct = default)
    {
        var response = await _trackService.UpdateAsync(dto, cancellationToken: ct);
        return Ok(response);
    }

    /// <summary>
    /// Удаляет трек.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] string id, CancellationToken ct = default)
    {
        var request = new DeleteRequest { Id = id };
        var response = await _trackService.DeleteAsync(request, cancellationToken: ct);
        return Ok(response);
    }

    /// <summary>
    /// Получает список треков по ID автора.
    /// </summary>
    [HttpGet("author/{authorId}")]
    public async Task<IActionResult> GetByAuthorIdAsync([FromRoute] string authorId, CancellationToken ct = default)
    {
        var request = new GetListByUserIdRequest { UserId = authorId };
        var response = await _trackService.GetListByUserIdAsync(request, cancellationToken: ct);
        return Ok(response);
    }
}