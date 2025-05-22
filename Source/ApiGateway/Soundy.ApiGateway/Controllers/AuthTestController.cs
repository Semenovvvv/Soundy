using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soundy.ApiGateway.Configurations;
using System.IdentityModel.Tokens.Jwt;

namespace Soundy.ApiGateway.Controllers;

[ApiController]
[Route("api/auth-test")]
public class AuthTestController : ControllerBase
{
    private readonly ILogger<AuthTestController> _logger;

    public AuthTestController(ILogger<AuthTestController> logger)
    {
        _logger = logger;
    }

    [HttpGet("public")]
    public IActionResult PublicEndpoint()
    {
        return Ok(new { message = "Это публичный эндпоинт, аутентификация не требуется." });
    }

    [HttpGet("secured")]
    [JwtAuthorize]
    public IActionResult SecuredEndpoint()
    {
        var userId = this.GetUserId();
        return Ok(new 
        { 
            message = "Это защищенный эндпоинт. Вы аутентифицированы.", 
            userId 
        });
    }

    [HttpGet("standard-auth")]
    [Authorize]
    public IActionResult StandardAuthEndpoint()
    {
        return Ok(new { message = "Это стандартный защищенный эндпоинт с атрибутом [Authorize]." });
    }

    [HttpGet("token-debug")]
    public IActionResult TokenDebug()
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        _logger.LogInformation($"Auth header: {authHeader}");

        string token;
        
        // Проверяем, начинается ли строка с "Bearer "
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            token = authHeader.Substring("Bearer ".Length).Trim();
            _logger.LogInformation("Получен токен с префиксом Bearer");
        }
        else if (!string.IsNullOrEmpty(authHeader))
        {
            // Если не начинается с "Bearer ", то предполагаем, что это сам токен
            token = authHeader.Trim();
            _logger.LogInformation("Получен токен без префикса Bearer");
        }
        else
        {
            return BadRequest(new { message = "Токен не предоставлен" });
        }
        
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (!tokenHandler.CanReadToken(token))
            {
                return BadRequest(new { 
                    message = "Неверный формат JWT токена", 
                    token = token,
                    tip = "Токен должен быть в формате JWT (например: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...)"
                });
            }

            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            var claims = jwtToken.Claims.Select(c => new { type = c.Type, value = c.Value }).ToList();
            
            var validFrom = jwtToken.ValidFrom;
            var validTo = jwtToken.ValidTo;
            var isExpired = DateTime.UtcNow > validTo;
            
            return Ok(new {
                message = "Информация о токене",
                tokenHeader = jwtToken.Header,
                claims,
                validFrom,
                validTo,
                isExpired,
                now = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при парсинге токена");
            return BadRequest(new { 
                message = $"Ошибка при парсинге токена: {ex.Message}",
                token = token
            });
        }
    }

    [HttpGet("auth-status")]
    public IActionResult AuthStatus()
    {
        var isAuthenticated = User?.Identity?.IsAuthenticated ?? false;
        
        // Создаем безопасный список claims
        var claimsList = new List<object>();
        if (User?.Claims != null) {
            claimsList = User.Claims.Select(c => new { type = c.Type, value = c.Value }).Cast<object>().ToList();
        }
        
        var userId = HttpContext.Items.ContainsKey("UserId") ? HttpContext.Items["UserId"]?.ToString() : null;

        return Ok(new {
            isAuthenticated,
            claims = claimsList,
            userId,
            hasValidToken = !string.IsNullOrEmpty(userId),
            headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())
        });
    }
} 