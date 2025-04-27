using Grpc.Core;
using System.Text.Json;

namespace Soundy.ApiGateway.Middlewares
{
    public class GrpcExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GrpcExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (RpcException ex)
            {
                await HandleRpcExceptionAsync(context, ex);
            }
        }

        private static Task HandleRpcExceptionAsync(HttpContext context, RpcException ex)
        {
            var statusCode = ex.StatusCode switch
            {
                StatusCode.InvalidArgument => StatusCodes.Status400BadRequest,
                StatusCode.Unauthenticated => StatusCodes.Status401Unauthorized,
                StatusCode.PermissionDenied => StatusCodes.Status403Forbidden,
                StatusCode.NotFound => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            var response = new
            {
                Success = false,
                Message = ex.Message,
                ErrorCode = ex.StatusCode.ToString()
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
