using Microsoft.AspNetCore.Mvc;

namespace Soundy.ApiGateway.Configurations;

public static class ControllerExtensions
{
    public static string GetUserId(this ControllerBase controller)
    {
        return controller.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
    }

    public static string GetJwtToken(this ControllerBase controller)
    {
        return controller.HttpContext.Items["JwtToken"]?.ToString() ?? string.Empty;
    }
} 