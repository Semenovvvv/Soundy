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

            var track = await dbContext.Tracks
                .AsNoTracking()
                .Include(t => t.LikedBy)
                .FirstOrDefaultAsync(x => x.Id == dto.Id, ct);

            if (track is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Track with Id = {dto.Id} not found"));

            var request = new Service.User.GetByIdRequest { Id = track.AuthorId.ToString() };
            var response = await _userService.GetByIdAsync(request, cancellationToken: ct);
            var author = _mapper.Map<User>(response.User);

            track.Author = author;

            var trackDto = _mapper.Map<TrackDto>(track);
            
            // Проверяем, лайкнул ли пользователь этот трек
            if (dto.UserId.HasValue)
            {
                trackDto.IsLiked = track.LikedBy.Any(lt => lt.UserId == dto.UserId.Value);
            }

            return new GetByIdResponseDto { Track = trackDto };
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

            var query = dbContext.Tracks
                .AsNoTracking()
                .Include(t => t.LikedBy)
                .Where(x => EF.Functions.ILike(x.Title, $"%{dto.Pattern}%"));

            var totalCount = await query.CountAsync(ct);
            var pageCount = (int)Math.Ceiling(totalCount / (double)dto.PageSize);

            var tracks = await query
                .Skip((dto.PageNum - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .ToListAsync(ct);

            var authorIds = tracks.Select(x => x.AuthorId).Distinct().ToList();
            var authors = new Dictionary<Guid, User>();

            foreach (var authorId in authorIds)
            {
                var request = new Service.User.GetByIdRequest { Id = authorId.ToString() };
                var response = await _userService.GetByIdAsync(request, cancellationToken: ct);
                var author = _mapper.Map<User>(response.User);
                authors[authorId] = author;
            }

            foreach (var track in tracks)
            {
                if (authors.TryGetValue(track.AuthorId, out var author))
                {
                    track.Author = author;
                }
            }

            var trackDtos = tracks.Select(track =>
            {
                var trackDto = _mapper.Map<TrackDto>(track);
                
                // Проверяем, лайкнул ли пользователь этот трек
                if (dto.UserId.HasValue)
                {
                    trackDto.IsLiked = track.LikedBy.Any(lt => lt.UserId == dto.UserId.Value);
                }
                
                return trackDto;
            }).ToList();

            return new SearchResponseDto
            {
                Pattern = dto.Pattern,
                PageSize = dto.PageSize,
                PageNum = dto.PageNum,
                PageCount = pageCount,
                TotalCount = totalCount,
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
        public async Task<GetListByUserIdResponseDto> GetListByUserId(GetListByUserIdRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var tracks = await dbContext.Tracks
                .Include(t => t.LikedBy)
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

            var trackDtos = tracks.Select(track =>
            {
                var trackDto = _mapper.Map<TrackDto>(track);
                
                // Проверяем, лайкнул ли пользователь этот трек
                trackDto.IsLiked = track.LikedBy.Any(lt => lt.UserId == dto.UserId);
                
                return trackDto;
            }).ToList();

            return new GetListByUserIdResponseDto()
            {
                Tracks = trackDtos
            };
        }

        /// <summary>
        /// Добавляет лайк треку от пользователя
        /// </summary>
        /// <param name="dto">Данные трека и пользователя</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Результат операции добавления лайка</returns>
        public async Task<LikeTrackResponseDto> LikeTrackAsync(LikeTrackRequestDto dto, CancellationToken ct = default)
        {
            try
            {
                await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

                // Проверяем существование трека
                var track = await dbContext.Tracks
                    .Include(t => t.LikedBy)
                    .FirstOrDefaultAsync(t => t.Id == dto.TrackId, ct);

                if (track == null)
                    throw new RpcException(new Status(StatusCode.NotFound, $"Track with Id = {dto.TrackId} not found"));

                // Проверяем, не лайкнул ли уже пользователь этот трек
                var existingLike = await dbContext.LikedTracks
                    .FirstOrDefaultAsync(lt => lt.TrackId == dto.TrackId && lt.UserId == dto.UserId, ct);

                if (existingLike != null)
                {
                    // Трек уже лайкнут пользователем
                    var trackDto = _mapper.Map<TrackDto>(track);
                    trackDto.IsLiked = true;
                    return new LikeTrackResponseDto { Success = true, Track = trackDto };
                }

                // Добавляем лайк
                var likedTrack = new LikedTrack
                {
                    TrackId = dto.TrackId,
                    UserId = dto.UserId,
                    LikedAt = DateTime.UtcNow,
                    Track = track
                };

                await dbContext.LikedTracks.AddAsync(likedTrack, ct);
                await dbContext.SaveChangesAsync(ct);

                // Загружаем информацию об авторе
                var request = new Service.User.GetByIdRequest { Id = track.AuthorId.ToString() };
                var response = await _userService.GetByIdAsync(request, cancellationToken: ct);
                var author = _mapper.Map<User>(response.User);

                track.Author = author;

                var resultTrackDto = _mapper.Map<TrackDto>(track);
                resultTrackDto.IsLiked = true;

                return new LikeTrackResponseDto { Success = true, Track = resultTrackDto };
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        /// <summary>
        /// Удаляет лайк трека от пользователя
        /// </summary>
        /// <param name="dto">Данные трека и пользователя</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Результат операции удаления лайка</returns>
        public async Task<UnlikeTrackResponseDto> UnlikeTrackAsync(UnlikeTrackRequestDto dto, CancellationToken ct = default)
        {
            try
            {
                await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

                // Проверяем существование трека
                var track = await dbContext.Tracks
                    .Include(t => t.LikedBy)
                    .FirstOrDefaultAsync(t => t.Id == dto.TrackId, ct);

                if (track == null)
                    throw new RpcException(new Status(StatusCode.NotFound, $"Track with Id = {dto.TrackId} not found"));

                // Находим существующий лайк
                var existingLike = await dbContext.LikedTracks
                    .FirstOrDefaultAsync(lt => lt.TrackId == dto.TrackId && lt.UserId == dto.UserId, ct);

                if (existingLike == null)
                {
                    // Лайк не найден
                    var trackDto = _mapper.Map<TrackDto>(track);
                    trackDto.IsLiked = false;
                    return new UnlikeTrackResponseDto { Success = true, Track = trackDto };
                }

                // Удаляем лайк
                dbContext.LikedTracks.Remove(existingLike);
                await dbContext.SaveChangesAsync(ct);

                // Загружаем информацию об авторе
                var request = new Service.User.GetByIdRequest { Id = track.AuthorId.ToString() };
                var response = await _userService.GetByIdAsync(request, cancellationToken: ct);
                var author = _mapper.Map<User>(response.User);

                track.Author = author;

                var resultTrackDto = _mapper.Map<TrackDto>(track);
                resultTrackDto.IsLiked = false;

                return new UnlikeTrackResponseDto { Success = true, Track = resultTrackDto };
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        /// <summary>
        /// Получает список лайкнутых треков пользователя
        /// </summary>
        /// <param name="dto">Идентификатор пользователя</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список лайкнутых треков</returns>
        public async Task<GetLikedTracksResponseDto> GetLikedTracksAsync(GetLikedTracksRequestDto dto, CancellationToken ct = default)
        {
            try
            {
                await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

                // Получаем все лайкнутые треки пользователя
                var likedTracks = await dbContext.LikedTracks
                    .Where(lt => lt.UserId == dto.UserId)
                    .Include(lt => lt.Track)
                    .OrderByDescending(lt => lt.LikedAt)
                    .ToListAsync(ct);

                // Получаем идентификаторы авторов треков
                var authorIds = likedTracks.Select(lt => lt.Track.AuthorId).Distinct().ToList();

                // Загружаем информацию об авторах
                var authors = new Dictionary<Guid, User>();
                foreach (var authorId in authorIds)
                {
                    var request = new Service.User.GetByIdRequest { Id = authorId.ToString() };
                    var response = await _userService.GetByIdAsync(request, cancellationToken: ct);
                    var author = _mapper.Map<User>(response.User);
                    authors[authorId] = author;
                }

                // Формируем список треков с авторами
                var tracks = likedTracks.Select(lt =>
                {
                    lt.Track.Author = authors[lt.Track.AuthorId];
                    var trackDto = _mapper.Map<TrackDto>(lt.Track);
                    trackDto.IsLiked = true;
                    return trackDto;
                }).ToList();

                return new GetLikedTracksResponseDto { Tracks = tracks };
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }
    }
}
