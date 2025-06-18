using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Soundy.ApiGateway.Configurations;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class JwtAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var logger = context.HttpContext.RequestServices.GetService<ILogger<JwtAuthorizeAttribute>>();
        
        logger?.LogInformation("JwtAuthorizeAttribute: Проверка авторизации...");
        
        // Проверяем наличие атрибута AllowAnonymous
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata
            .Any(em => em.GetType() == typeof(AllowAnonymousAttribute));

        if (allowAnonymous)
        {
            logger?.LogInformation("JwtAuthorizeAttribute: Обнаружен AllowAnonymous, пропускаем проверку");
            return;
        }

        // Проверяем, есть ли аутентифицированный пользователь
        var user = context.HttpContext.User;
        if (user.Identity is { IsAuthenticated: false })
        {
            logger?.LogWarning("JwtAuthorizeAttribute: Пользователь не аутентифицирован");
            
            // Не аутентифицирован
            context.Result = new UnauthorizedResult();
            return;
        }
        
        var userId = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId))
        {
            logger?.LogWarning("JwtAuthorizeAttribute: UserId пустой");
            context.Result = new UnauthorizedResult();
            return;
        }

        logger?.LogInformation($"JwtAuthorizeAttribute: Авторизация успешна для пользователя {userId}");
    }
} 