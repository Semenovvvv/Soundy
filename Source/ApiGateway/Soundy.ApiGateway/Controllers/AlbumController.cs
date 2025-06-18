using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Service.Album;
using Soundy.ApiGateway.Configurations;

namespace Soundy.ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlbumController : ControllerBase
    {
        private readonly AlbumGrpcService.AlbumGrpcServiceClient _albumService;

        public AlbumController(AlbumGrpcService.AlbumGrpcServiceClient albumService)
        {
            _albumService = albumService;
        }

        [HttpPost]
        [JwtAuthorize]
        public async Task<IActionResult> CreateAsync([FromBody] CreateRequest request, [EnumeratorCancellation] CancellationToken ct)
        {
            var response = await _albumService.CreateAsync(request, cancellationToken: ct);
            return Ok(response);
        }

        [HttpPost("{albumId}")]
        [JwtAuthorize]
        public async Task<IActionResult> AddTrackAsync(string albumId, [FromBody] string trackId, [EnumeratorCancellation] CancellationToken ct)
        {
            var request = new AddTrackRequest()
            {
                Id = albumId,
                TrackId = trackId
            };
            var response = await _albumService.AddTrackAsync(request, cancellationToken: ct);
            return Ok(response);
        }

        /// <summary>
        /// Получает все альбомы пользователя по ID автора.
        /// </summary>
        [HttpGet("author/{authorId}")]
        [JwtAuthorize]
        public async Task<IActionResult> GetByAuthorIdAsync([FromRoute] string authorId, CancellationToken ct = default)
        {
            var request = new GetByAuthorIdRequest { AuthorId = authorId };
            var response = await _albumService.GetByAuthorIdAsync(request, cancellationToken: ct);
            return Ok(response);
        }

        [HttpGet("{id}")]
        [JwtAuthorize]
        public async Task<IActionResult> GetByIdAsync([FromRoute] string id, CancellationToken ct = default)
        {
            var request = new GetByIdRequest { Id = id };
            var response = await _albumService.GetByIdAsync(request, cancellationToken: ct);
            return Ok(response);
        }

        /// <summary>
        /// Выполняет поиск альбомов по названию с пагинацией
        /// </summary>
        /// <param name="pattern">Строка поиска</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <param name="pageNum">Номер страницы</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список альбомов, соответствующих критериям поиска</returns>
        [HttpGet("search")]
        [JwtAuthorize]
        public async Task<IActionResult> SearchAsync([FromQuery] string pattern, [FromQuery] int pageSize = 10, [FromQuery] int pageNum = 1, CancellationToken ct = default)
        {
            var request = new SearchRequest
            {
                Pattern = pattern,
                PageSize = pageSize,
                PageNum = pageNum
            };

            var response = await _albumService.SearchAsync(request, cancellationToken: ct);
            return Ok(response);
        }

        /// <summary>
        /// Получает список последних созданных альбомов
        /// </summary>
        /// <param name="count">Количество записей для получения (по умолчанию 10)</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список последних созданных альбомов</returns>
        [HttpGet("latest")]
        [JwtAuthorize]
        public async Task<IActionResult> GetLatestAlbums([FromQuery] int count = 10, CancellationToken ct = default)
        {
            var request = new GetLatestAlbumsRequest { Count = count };
            var response = await _albumService.GetLatestAlbumsAsync(request, cancellationToken: ct);
            return Ok(response);
        }
    }
}
