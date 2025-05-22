using AutoMapper;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Service.User;
using Soundy.CatalogService.DataAccess;
using Soundy.CatalogService.Dto;
using Soundy.CatalogService.Dto.TrackDtos;
using Soundy.CatalogService.Entities;
using Soundy.CatalogService.Interfaces;

namespace Soundy.CatalogService.Services
{
    public class TrackService : ITrackService
    {
        private readonly IDbContextFactory<DatabaseContext> _dbFactory;
        private readonly UserGrpcService.UserGrpcServiceClient _userService;
        private readonly ILogger<TrackService> _logger;
        private readonly IMapper _mapper;

        public TrackService(
            IDbContextFactory<DatabaseContext> dbFactory, 
            UserGrpcService.UserGrpcServiceClient userService,
            ILogger<TrackService> logger, 
            IMapper mapper)
        {
            _dbFactory = dbFactory;
            _userService = userService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Создает новый трек и загружает информацию об авторе
        /// </summary>
        /// <param name="dto">Данные для создания трека</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Информация о созданном треке</returns>
        public async Task<CreateResponseDto> CreateAsync(CreateRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var track = new Track()
            {
                AlbumId = dto.AlbumId,
                AuthorId = dto.AuthorId,
                AvatarUrl = dto.AvatarUrl,
                Title = dto.Title,
                Duration = dto.Duration,
                CreatedAt = DateTime.UtcNow
            };

            await dbContext.Tracks.AddAsync(track, ct);
            await dbContext.SaveChangesAsync(ct);
            
            try
            {
                var userResponse = await _userService.GetByIdAsync(new GetByIdRequest { Id = track.AuthorId.ToString() }, cancellationToken: ct);
                if (userResponse?.User != null)
                {
                    track.Author = _mapper.Map<User>(userResponse.User);
                    _logger.LogInformation("Successfully loaded author information for newly created track {TrackId}", track.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load author information for newly created track {TrackId}", track.Id);
            }

            return new CreateResponseDto
            {
                Track = _mapper.Map<TrackDto>(track)
            };
        }

        /// <summary>
        /// Получает трек по идентификатору с информацией об авторе
        /// </summary>
        /// <param name="dto">Запрос с идентификатором трека</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Информация о треке с данными автора</returns>
        public async Task<GetByIdResponseDto> GetByIdAsync(GetByIdRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var track = await dbContext.Tracks.FirstOrDefaultAsync(x => x.Id == dto.Id, ct);

            if (track is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Track with Id {dto.Id} not found"));

            try
            {
                var userResponse = await _userService.GetByIdAsync(new GetByIdRequest { Id = track.AuthorId.ToString() }, cancellationToken: ct);
                if (userResponse?.User != null)
                {
                    track.Author = _mapper.Map<User>(userResponse.User);
                    _logger.LogInformation("Successfully loaded author information for track {TrackId}", track.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load author information for track {TrackId}", track.Id);
            }

            return new GetByIdResponseDto
            {
                Track = _mapper.Map<TrackDto>(track)
            };
        }

        /// <summary>
        /// Получает список треков по идентификатору плейлиста
        /// </summary>
        /// <param name="dto">Запрос с идентификатором плейлиста</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список треков плейлиста</returns>
        public async Task<GetListByPlaylistResponseDto> GetListByPlaylistAsync(GetListByPlaylistRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var playlist = await dbContext.Playlists.Include(playlist => playlist.Tracks).FirstOrDefaultAsync(x => x.Id == dto.PlaylistId, ct);

            if (playlist is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Playlist with Id {dto.PlaylistId} not found"));

            var tracks = playlist.Tracks
                .Select(x => _mapper.Map<TrackDto>(x.Track))
                .ToList();

            return new GetListByPlaylistResponseDto()
            {
                PlaylistId = dto.PlaylistId,
                Playlist = _mapper.Map<PlaylistDto>(playlist),
                Tracks = tracks
            };
        }

        /// <summary>
        /// Выполняет поиск треков по строке запроса с загрузкой информации об авторах
        /// </summary>
        /// <param name="dto">Параметры поиска</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Результаты поиска с информацией об авторах</returns>
        public async Task<SearchResponseDto> SearchAsync(SearchRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var tracksQuery = dbContext.Tracks
                .Where(t => EF.Functions.Like(t.Title, $"%{dto.Pattern}%"))
                .Skip((dto.PageNum - 1) * dto.PageSize)
                .Take(dto.PageSize);
            
            var tracks = await tracksQuery.ToListAsync(ct);
            
            foreach (var track in tracks)
            {
                try
                {
                    var userResponse = await _userService.GetByIdAsync(new GetByIdRequest { Id = track.AuthorId.ToString() }, cancellationToken: ct);
                    if (userResponse?.User != null)
                    {
                        track.Author = _mapper.Map<User>(userResponse.User);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to load author information for track {TrackId}", track.Id);
                }
            }

            var trackDtos = tracks.Select(x => _mapper.Map<TrackDto>(x)).ToList();

            return new SearchResponseDto()
            {
                Pattern = dto.Pattern,
                PageNum = dto.PageNum,
                PageSize = dto.PageSize,
                Tracks = trackDtos
            };
        }

        /// <summary>
        /// Обновляет информацию о треке и загружает данные автора
        /// </summary>
        /// <param name="dto">Данные для обновления</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Обновленная информация о треке</returns>
        public async Task<UpdateResponseDto> UpdateAsync(UpdateRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var track = await dbContext.Tracks.FirstOrDefaultAsync(x => x.Id == dto.Id, ct);

            if (track is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Track with Id {dto.Id} not found"));

            track.Title = dto.Title;

            await dbContext.SaveChangesAsync(ct);
            
            try
            {
                var userResponse = await _userService.GetByIdAsync(new GetByIdRequest { Id = track.AuthorId.ToString() }, cancellationToken: ct);
                if (userResponse?.User != null)
                {
                    track.Author = _mapper.Map<User>(userResponse.User);
                    _logger.LogInformation("Successfully loaded author information for updated track {TrackId}", track.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load author information for updated track {TrackId}", track.Id);
            }
            
            return new UpdateResponseDto()
            {
                Track = _mapper.Map<TrackDto>(track)
            };
        }

        /// <summary>
        /// Удаляет трек из системы
        /// </summary>
        /// <param name="dto">Идентификатор удаляемого трека</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Результат операции удаления</returns>
        public async Task<DeleteResponseDto> DeleteAsync(DeleteRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var track = await dbContext.Tracks.FirstOrDefaultAsync(x => x.Id == dto.Id, ct);

            if (track is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Track with Id {dto.Id} not found"));

            await dbContext.Tracks
                .Where(x => x.Id == dto.Id)
                .ExecuteDeleteAsync(ct);
            return new DeleteResponseDto()
            {
                Success = true
            };
        }

        /// <summary>
        /// Получает список треков по идентификатору пользователя
        /// </summary>
        /// <param name="dto">Идентификатор пользователя</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список треков пользователя</returns>
        public async Task<GetListByUserIdResponseDto> GetListByUserIdRequest(GetListByUserIdRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var tracks = await dbContext.Tracks
                .Where(x => x.AuthorId == dto.UserId)
                .ToListAsync(ct);
            
            User author = null;
            try 
            {
                var userResponse = await _userService.GetByIdAsync(new GetByIdRequest { Id = dto.UserId.ToString() }, cancellationToken: ct);
                if (userResponse?.User != null)
                {
                    author = _mapper.Map<User>(userResponse.User);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load author information for tracks by user {UserId}", dto.UserId);
            }
            
            if (author != null)
            {
                foreach (var track in tracks)
                {
                    track.Author = author;
                }
            }

            var trackDtos = tracks.Select(x => _mapper.Map<TrackDto>(x)).ToList();

            return new GetListByUserIdResponseDto()
            {
                Tracks = trackDtos
            };
        }
    }
}
