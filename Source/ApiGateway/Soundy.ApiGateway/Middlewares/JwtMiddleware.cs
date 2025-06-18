using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Service.Iam;
using Soundy.ApiGateway.Configurations;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Soundy.ApiGateway.Middlewares;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JwtConfig _jwtConfig;
    private readonly ILogger<JwtMiddleware> _logger;

    public JwtMiddleware(RequestDelegate next, IOptions<JwtConfig> jwtOptions, 
        ILogger<JwtMiddleware> logger)
    {
        _next = next;
        _jwtConfig = jwtOptions.Value;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IAMGrpcService.IAMGrpcServiceClient iamClient)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        _logger.LogInformation($"Auth header: {authHeader}");

        string token = null;

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            token = authHeader.Substring("Bearer ".Length).Trim();
            _logger.LogInformation("Получен токен с префиксом Bearer");
        }
        else if (!string.IsNullOrEmpty(authHeader))
        {
            token = authHeader.Trim();
            _logger.LogInformation("Получен токен без префикса Bearer");
        }

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                await AttachUserToContext(context, iamClient, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "JWT middleware error");
                // Продолжаем выполнение, токен не будет валидным, но будет установлен статус 401
                // в AuthorizeAttribute или других местах проверки авторизации
            }
        }
        else
        {
            _logger.LogInformation("No token found in Authorization header");
        }

        await _next(context);
    }

    private async Task AttachUserToContext(HttpContext context, IAMGrpcService.IAMGrpcServiceClient iamClient, string token)
    {
        try
        {
            _logger.LogInformation("Validating token...");
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            // Проводим базовую валидацию формата токена
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtConfig.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtConfig.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            _logger.LogInformation("Token format validated, requesting IAM service validation");

            // Запрос в IAM сервис для дополнительной валидации
            var validationResponse = await iamClient.ValidateTokenAsync(new ValidateTokenRequest
            {
                Token = token
            });

            if (validationResponse.IsValid)
            {
                _logger.LogInformation($"Token validated by IAM service. User ID: {validationResponse.UserId}");
                
                // Установка ClaimsPrincipal
                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = validationResponse.UserId;

                var identity = new System.Security.Claims.ClaimsIdentity(jwtToken.Claims, "JWT");
                context.User = new System.Security.Claims.ClaimsPrincipal(identity);

                // Добавляем информацию о пользователе в HttpContext.Items для легкого доступа
                context.Items["UserId"] = userId;
                context.Items["JwtToken"] = token;
            }
            else
            {
                _logger.LogWarning($"Token rejected by IAM service: {validationResponse.UserId}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating JWT token");
            throw; // re-throw for handling in InvokeAsync
        }
    }
} 