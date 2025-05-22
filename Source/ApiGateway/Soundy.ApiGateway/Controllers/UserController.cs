using Microsoft.AspNetCore.Mvc;
using Service.User;
using Soundy.ApiGateway.Configurations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Soundy.ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserGrpcService.UserGrpcServiceClient _client;
        private readonly ILogger<UserController> _logger;

        public UserController(
            UserGrpcService.UserGrpcServiceClient client,
            ILogger<UserController> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Создает нового пользователя.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateRequest dto,
            CancellationToken ct = default)
        {
            var response = await _client.CreateAsync(dto, cancellationToken: ct);
            return Ok(response);
        }

        /// <summary>
        /// Получает пользователя по ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById([FromRoute] string id, CancellationToken ct = default)
        {
            var request = new GetByIdRequest { Id = id };
            var response = await _client.GetByIdAsync(request, cancellationToken: ct);
            return Ok(response);
            
        }

        /// <summary>
        /// Обновляет данные пользователя.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateRequest dto, CancellationToken ct = default)
        {
            var response = await _client.UpdateAsync(dto, cancellationToken: ct);
            return Ok(response);
        }

        /// <summary>
        /// Удаляет пользователя.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] string id, CancellationToken ct = default)
        {
            var request = new DeleteRequest() { Id = id };
            var response = await _client.DeleteAsync(request, cancellationToken: ct);
            return Ok(response);
        }

        /// <summary>
        /// Поиск пользователей по фильтру.
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] SearchRequest dto, CancellationToken ct = default)
        {
            var response = await _client.SearchAsync(dto, cancellationToken: ct);
            return Ok(response);
        }

        /// <summary>
        /// Получает информацию о текущем аутентифицированном пользователе по JWT токену
        /// </summary>
        /// <returns>Информация о пользователе</returns>
        [HttpGet("me")]
        [JwtAuthorize]
        public async Task<IActionResult> GetCurrentUser(CancellationToken ct = default)
        {
            try
            {
                // Получаем ID пользователя из JWT токена с помощью метода расширения GetUserId
                var userId = this.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("No user ID found in JWT token");
                    return Unauthorized(new { message = "User not authenticated or token does not contain user ID" });
                }

                _logger.LogInformation("Requesting user data for ID: {UserId}", userId);

                try
                {
                    // Запрашиваем информацию о пользователе из User сервиса
                    var userResponse = await _client.GetByIdAsync(new GetByIdRequest { Id = userId }, cancellationToken: ct);

                    // Проверяем, что получили информацию о пользователе
                    if (userResponse?.User == null)
                    {
                        _logger.LogWarning("User with ID {UserId} not found in User service", userId);
                        return NotFound(new { message = "User not found" });
                    }

                    // Формируем объект с данными пользователя
                    var userDto = new UserProfileDto
                    {
                        Id = userResponse.User.Id,
                        Name = userResponse.User.Name,
                        Email = userResponse.User.Email,
                        Bio = userResponse.User.Bio,
                        AvatarUrl = userResponse.User.AvatarUrl ?? "",
                        CreatedAt = userResponse.User.CreatedAt?.ToDateTime() ?? DateTime.MinValue
                    };

                    _logger.LogInformation("Successfully retrieved user data for ID: {UserId}", userId);
                    return Ok(userDto);
                }
                catch (Grpc.Core.RpcException ex) when (ex.Status.StatusCode == Grpc.Core.StatusCode.NotFound)
                {
                    _logger.LogWarning(ex, "User with ID {UserId} not found in User service", userId);
                    return NotFound(new { message = "User not found in User service" });
                }
                catch (Grpc.Core.RpcException ex)
                {
                    _logger.LogError(ex, "gRPC error when retrieving user data for ID: {UserId}", userId);
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, new 
                    { 
                        message = "User service is unavailable", 
                        details = ex.Status.Detail
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving user information");
                return StatusCode(500, new { message = "Failed to retrieve user information", details = ex.Message });
            }
        }
    }

    /// <summary>
    /// DTO для профиля пользователя
    /// </summary>
    public class UserProfileDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("bio")]
        public string Bio { get; set; }

        [JsonPropertyName("avatarUrl")]
        public string AvatarUrl { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}
