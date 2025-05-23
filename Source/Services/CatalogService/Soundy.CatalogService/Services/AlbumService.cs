using AutoMapper;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Service.User;
using Soundy.CatalogService.DataAccess;
using Soundy.CatalogService.Dto;
using Soundy.CatalogService.Dto.AlbumDtos;
using Soundy.CatalogService.Entities;
using Soundy.CatalogService.Interfaces;

namespace Soundy.CatalogService.Services
{
    public class AlbumService : IAlbumService
    {
        private readonly IDbContextFactory<DatabaseContext> _dbFactory;
        private readonly UserGrpcService.UserGrpcServiceClient _userService;
        private readonly ILogger<AlbumService> _logger;
        private readonly IMapper _mapper;

        public AlbumService(
            IDbContextFactory<DatabaseContext> dbFactory, 
            UserGrpcService.UserGrpcServiceClient userService,
            ILogger<AlbumService> logger, 
            IMapper mapper)
        {
            _dbFactory = dbFactory;
            _userService = userService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Создает новый альбом и загружает информацию об авторе
        /// </summary>
        /// <param name="dto">Данные для создания альбома</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Информация о созданном альбоме</returns>
        public async Task<CreateResponseDto> CreateAsync(CreateRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);
            var album = new Album()
            {
                Id = Guid.NewGuid(),
                AuthorId = dto.AuthorId,
                Title = dto.Title,
                CreatedAt = DateTime.UtcNow,
                AvatarUrl = dto.AvatarUrl
            };

            await dbContext.Albums.AddAsync(album, ct);
            await dbContext.SaveChangesAsync(ct);
            
            try
            {
                var userResponse = await _userService.GetByIdAsync(new GetByIdRequest { Id = album.AuthorId.ToString() }, cancellationToken: ct);
                if (userResponse?.User != null)
                {
                    album.Author = _mapper.Map<User>(userResponse.User);
                    _logger.LogInformation("Successfully loaded author information for album {AlbumId}", album.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load author information for album {AlbumId}", album.Id);
            }
            
            return new CreateResponseDto()
            {
                Album = _mapper.Map<AlbumDto>(album)
            };
        }

        /// <summary>
        /// Добавляет трек в альбом и обновляет информацию об авторе
        /// </summary>
        /// <param name="dto">Данные трека и альбома</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Обновленная информация об альбоме</returns>
        public async Task<AddTrackResponseDto> AddTrackAsync(AddTrackRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var album = await dbContext.Albums.Include(album => album.Tracks).FirstOrDefaultAsync(x => x.Id == dto.Id, ct);
            if (album == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Album with Id {dto.Id} not found"));
                
            var track = await dbContext.Tracks.FirstOrDefaultAsync(x => x.Id == dto.TrackId, ct);
            if (track == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Track with Id {dto.TrackId} not found"));
            
            album.Tracks.Add(track);
            await dbContext.SaveChangesAsync(ct);
            
            try
            {
                var userResponse = await _userService.GetByIdAsync(new GetByIdRequest { Id = album.AuthorId.ToString() }, cancellationToken: ct);
                if (userResponse?.User != null)
                {
                    album.Author = _mapper.Map<User>(userResponse.User);
                    _logger.LogInformation("Successfully loaded author information for album {AlbumId} after adding track", album.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load author information for album {AlbumId} after adding track", album.Id);
            }
            
            return new AddTrackResponseDto()
            {
                Album = _mapper.Map<AlbumDto>(album)
            };
        }
        
        /// <summary>
        /// Получает информацию об альбоме по его идентификатору
        /// </summary>
        /// <param name="dto">Идентификатор альбома</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Детальная информация об альбоме с данными автора</returns>
        public async Task<GetByIdResponseDto> GetByIdAsync(GetByIdRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var album = await dbContext.Albums
                .Include(a => a.Tracks)
                .FirstOrDefaultAsync(x => x.Id == dto.Id, ct);

            if (album == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Album with Id {dto.Id} not found"));

            try
            {
                var userResponse = await _userService.GetByIdAsync(new GetByIdRequest { Id = album.AuthorId.ToString() }, cancellationToken: ct);
                if (userResponse?.User != null)
                {
                    album.Author = _mapper.Map<User>(userResponse.User);
                    _logger.LogInformation("Successfully loaded author information for album {AlbumId}", album.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load author information for album {AlbumId}", album.Id);
            }

            return new GetByIdResponseDto
            {
                Album = _mapper.Map<AlbumDto>(album)
            };
        }

        /// <summary>
        /// Получает список альбомов по идентификатору автора
        /// </summary>
        /// <param name="dto">Идентификатор автора</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список альбомов автора</returns>
        public async Task<GetByAuthorIdResponseDto> GetByAuthorIdAsync(GetByAuthorIdRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var albums = await dbContext.Albums
                .Include(a => a.Tracks)
                .Where(a => a.AuthorId == dto.AuthorId)
                .ToListAsync(ct);

            try
            {
                var userResponse = await _userService.GetByIdAsync(new GetByIdRequest { Id = dto.AuthorId.ToString() }, cancellationToken: ct);
                if (userResponse?.User != null)
                {
                    var author = _mapper.Map<User>(userResponse.User);
                    foreach (var album in albums)
                    {
                        album.Author = author;
                    }
                    _logger.LogInformation("Successfully loaded author information for albums by author {AuthorId}", dto.AuthorId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load author information for albums by author {AuthorId}", dto.AuthorId);
            }

            return new GetByAuthorIdResponseDto
            {
                Albums = albums.Select(a => _mapper.Map<AlbumDto>(a))
            };
        }
    }
}
