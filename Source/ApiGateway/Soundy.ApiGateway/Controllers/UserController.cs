using Microsoft.AspNetCore.Mvc;
using Service.User;
using Soundy.ApiGateway.Configurations;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Soundy.ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserGrpcService.UserGrpcServiceClient _client;
        private readonly ILogger<UserController> _logger;

        public UserController(UserGrpcService.UserGrpcServiceClient client, ILogger<UserController> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Создает нового пользователя.
        /// </summary>
        [HttpPost]
        [JwtAuthorize]
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
        [JwtAuthorize]
        public async Task<IActionResult> GetUserById([FromRoute] string id, CancellationToken ct = default)
        {
            var request = new GetByIdRequest { Id = id };
            var response = await _client.GetByIdAsync(request, cancellationToken: ct);
            return Ok(response);
            
        }

        /// <summary>
        /// Обновляет данные пользователя.
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <param name="dto">Данные для обновления</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Обновленные данные пользователя</returns>
        [HttpPut("{id}")]
        [JwtAuthorize]
        public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] UserUpdateDto dto, CancellationToken ct = default)
        {
            try
            {
                // Проверка модели происходит автоматически благодаря атрибутам валидации
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Некорректные данные", errors = ModelState });
                }

                // Проверяем, совпадает ли ID из маршрута с ID из токена (можно обновить только свой профиль)
                var userId = this.GetUserId();
                if (userId != id)
                {
                    _logger.LogWarning("Attempted unauthorized profile update: {ActualUserId} tried to update {TargetUserId}", userId, id);
                    return new ForbidResult();
                }

                var request = new UpdateRequest
                {
                    Id = id,
                    Username = dto.Name
                };

                // Добавляем Bio, AvatarUrl и Email, если они указаны
                if (dto.Bio != null)
                {
                    request.Bio = dto.Bio;
                }

                if (dto.AvatarUrl != null)
                {
                    request.AvatarUrl = dto.AvatarUrl;
                }

                _logger.LogInformation("Updating user profile for ID: {UserId}", id);
                var response = await _client.UpdateAsync(request, cancellationToken: ct);
                
                return Ok(new { 
                    message = "Профиль пользователя успешно обновлен",
                    user = new {
                        id = response.User.Id,
                        name = response.User.Name,
                        email = response.User.Email,
                        bio = response.User.Bio,
                        avatarUrl = response.User.AvatarUrl,
                        createdAt = response.User.CreatedAt?.ToDateTime()
                    }
                });
            }
            catch (Grpc.Core.RpcException ex) when (ex.Status.StatusCode == Grpc.Core.StatusCode.NotFound)
            {
                _logger.LogWarning(ex, "User with ID {UserId} not found", id);
                return NotFound(new { message = "Пользователь не найден" });
            }
            catch (Grpc.Core.RpcException ex) when (ex.Status.StatusCode == Grpc.Core.StatusCode.InvalidArgument)
            {
                _logger.LogWarning(ex, "Invalid argument when updating user {UserId}: {Message}", id, ex.Status.Detail);
                return BadRequest(new { message = ex.Status.Detail });
            }
            catch (Grpc.Core.RpcException ex)
            {
                _logger.LogError(ex, "gRPC error when updating user {UserId}: {Code} - {Message}", 
                    id, ex.Status.StatusCode, ex.Status.Detail);
                return StatusCode(StatusCodes.Status503ServiceUnavailable, 
                    new { message = "Сервис пользователей недоступен", details = ex.Status.Detail });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", id);
                return StatusCode(500, new { message = "Не удалось обновить профиль пользователя" });
            }
        }

        /// <summary>
        /// Удаляет пользователя.
        /// </summary>
        [HttpDelete("{id}")]
        [JwtAuthorize]
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
        [JwtAuthorize]
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

        /// <summary>
        /// Получает список последних зарегистрированных пользователей
        /// </summary>
        /// <param name="count">Количество записей для получения (по умолчанию 10)</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список последних зарегистрированных пользователей</returns>
        [HttpGet("latest")]
        [JwtAuthorize]
        public async Task<IActionResult> GetLatestUsers([FromQuery] int count = 10, CancellationToken ct = default)
        {
            var request = new GetLatestUsersRequest { Count = count };
            var response = await _client.GetLatestUsersAsync(request, cancellationToken: ct);
            return Ok(response);
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

    /// <summary>
    /// DTO для обновления пользователя
    /// </summary>
    public class UserUpdateDto
    {
        /// <summary>
        /// Имя пользователя (обязательное поле)
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Имя должно содержать от 2 до 100 символов")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Краткая информация о пользователе
        /// </summary>
        [StringLength(500, ErrorMessage = "Информация не должна превышать 500 символов")]
        [JsonPropertyName("bio")]
        public string? Bio { get; set; }

        /// <summary>
        /// URL аватара пользователя
        /// </summary>
        [JsonPropertyName("avatarUrl")]
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// Email пользователя (не используется при обновлении)
        /// </summary>
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }
}
