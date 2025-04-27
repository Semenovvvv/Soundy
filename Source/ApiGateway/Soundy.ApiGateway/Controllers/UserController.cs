using Microsoft.AspNetCore.Mvc;
using Soundy.SharedLibrary.Contracts.User;

namespace Soundy.ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserGrpcService.UserGrpcServiceClient _client;

        public UserController(UserGrpcService.UserGrpcServiceClient client)
        {
            _client = client;
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
    }
}
