using AutoMapper;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Service.User;
using Soundy.CatalogService.DataAccess;
using Soundy.CatalogService.Dto;
using Soundy.CatalogService.Dto.PlaylistDtos;
using Soundy.CatalogService.Entities;
using Soundy.CatalogService.Interfaces;

namespace Soundy.CatalogService.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IDbContextFactory<DatabaseContext> _dbFactory;
        private readonly UserGrpcService.UserGrpcServiceClient _userService;
        private readonly IMapper _mapper;

        public PlaylistService(IDbContextFactory<DatabaseContext> dbFactory, UserGrpcService.UserGrpcServiceClient userService, IMapper mapper)
        {
            _dbFactory = dbFactory;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<CreateResponseDto> CreateAsync(CreateRequestDto dto, CancellationToken ct = default)
        {
            try
            {
                await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

                

                await _userService.GetByIdAsync(new GetByIdRequest() { Id = dto.AuthorId.ToString() }, cancellationToken: ct);

                var playlist = new Playlist()
                {
                    AuthorId = dto.AuthorId,
                    Title = dto.Name,
                    CreatedAt = DateTime.UtcNow,
                    IsFavorite = false
                };

                await dbContext.Playlists.AddAsync(playlist, ct);
                await dbContext.SaveChangesAsync(ct);

                var request = new Service.User.GetByIdRequest { Id = dto.AuthorId.ToString() };
                var response = await _userService.GetByIdAsync(request);
                var author = _mapper.Map<User>(response.User);

                playlist.Author = author;

                return new CreateResponseDto()
                {
                    Playlist = _mapper.Map<PlaylistDto>(playlist)
                };
            }
            catch (RpcException prcEx)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public async Task<CreateFavoriteResponseDto> CreateFavoriteAsync(CreateFavoriteRequestDto dto, CancellationToken ct = default)
        {
            try
            {
                await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

                var playlist = new Playlist()
                {
                    AuthorId = dto.AuthorId,
                    Title = "Favorite",
                    CreatedAt = DateTime.UtcNow,
                    IsFavorite = true,
                    AvatarUrl = ""
                };

                await dbContext.Playlists.AddAsync(playlist, ct);
                await dbContext.SaveChangesAsync(ct);

                playlist.Author = new User()
                {
                    AvatarUrl = "",
                    Bio = "",
                    Email = "",
                    Name = "",
                    CreatedAt = DateTime.UtcNow
                };

                playlist.Tracks = new List<PlaylistTrack>();

                return new CreateFavoriteResponseDto{ Playlist = _mapper.Map<PlaylistDto>(playlist) };
            }
            catch (RpcException prcEx)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public async Task<GetByIdResponseDto> GetByIdAsync(GetByIdRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var playlist = await dbContext.Playlists
                .AsNoTracking()
                .Include(x => x.Tracks)
                .ThenInclude(x => x.Track)
                .FirstOrDefaultAsync(x => x.Id == dto.Id, ct);

            if (playlist is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Playlist with Id = {dto.Id} not found"));

            var request = new Service.User.GetByIdRequest { Id = playlist.AuthorId.ToString() };
            var response = await _userService.GetByIdAsync(request);
            var author = _mapper.Map<User>(response.User);

            playlist.Author = author;

            return new GetByIdResponseDto()
            {
                Playlist = _mapper.Map<PlaylistDto>(playlist)
            };
        }

        public async Task<GetListByAuthorIdResponseDto> GetListByAuthorIdAsync(GetListByAuthorIdRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var playlists = await dbContext.Playlists
                .AsNoTracking()
                .Where(x => x.AuthorId == dto.AuthorId)
                .IgnoreAutoIncludes()
                .ToListAsync(ct);

            var request = new Service.User.GetByIdRequest { Id = dto.AuthorId.ToString() };
            var response = await _userService.GetByIdAsync(request);
            var author = _mapper.Map<User>(response.User);

            foreach (var playlist in playlists)
            {
                playlist.Author = author;
            }

            return new GetListByAuthorIdResponseDto()
            {
                Playlists = _mapper.Map<IList<PlaylistDto>>(playlists)
            };
        }

        public async Task<GetFavoriteResponseDto> GetFavoriteAsync(GetFavoriteRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var playlist = await dbContext.Playlists
                .AsNoTracking()
                .Include(x => x.Tracks)
                .ThenInclude(x => x.Track)
                .FirstOrDefaultAsync(x => x.AuthorId == dto.AuthorId && x.IsFavorite, ct);

            if (playlist is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Playlist with Author Id = {dto.AuthorId} not found"));

            var request = new Service.User.GetByIdRequest { Id = dto.AuthorId.ToString() };
            var response = await _userService.GetByIdAsync(request);
            var author = _mapper.Map<User>(response.User);

            playlist.Author = author;

            return new GetFavoriteResponseDto { Playlist = _mapper.Map<PlaylistDto>(playlist) };
        }

        public async Task<AddTrackResponseDto> AddTrackAsync(AddTrackRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var playlist = await dbContext.Playlists
                               .Include(x => x.Tracks)
                               .ThenInclude(x => x.Track)
                               .FirstOrDefaultAsync(x => x.Id == dto.PlaylistId, ct) ??
                           throw new RpcException(new Status(StatusCode.NotFound, $"Playlist with id {dto.PlaylistId} not found"));

            var track = await dbContext.Tracks.FirstOrDefaultAsync(x => x.Id == dto.TrackId, ct)
                        ?? throw new RpcException(new Status(StatusCode.NotFound, $"Track with id {dto.TrackId} not found"));

            var pt = new PlaylistTrack()
            {
                Playlist = playlist,
                Track = track,
                AddedDate = DateTime.UtcNow
            };

            playlist.Tracks.Add(pt);

            await dbContext.SaveChangesAsync(ct);

            var request = new Service.User.GetByIdRequest { Id = playlist.AuthorId.ToString() };
            var response = await _userService.GetByIdAsync(request);
            var author = _mapper.Map<User>(response.User);

            playlist.Author = author;

            return new AddTrackResponseDto() { Playlist = _mapper.Map<PlaylistDto>(playlist) };
        }

        public async Task<UpdateResponseDto> UpdateAsync(UpdateRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var playlist = await dbContext.Playlists.FirstOrDefaultAsync(x => x.Id == dto.Id, ct);

            if (playlist is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Playlist with Id = {dto.Id} not found"));
            }

            playlist.Title = dto.Name;
            await dbContext.SaveChangesAsync(ct);

            var request = new Service.User.GetByIdRequest { Id = playlist.AuthorId.ToString() };
            var response = await _userService.GetByIdAsync(request);
            var author = _mapper.Map<User>(response.User);

            playlist.Author = author;

            return new UpdateResponseDto()
            {
                Playlist = _mapper.Map<PlaylistDto>(playlist)
            };
        }

        public async Task<DeleteResponseDto> DeleteAsync(DeleteRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var playlist = await dbContext.Playlists.FirstOrDefaultAsync(x => x.Id == dto.Id, ct);

            if (playlist is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Playlist with Id = {dto.Id} not found"));
            }

            await dbContext.Playlists
                .Where(x => x.Id == dto.Id)
                .ExecuteDeleteAsync(ct);

            return new DeleteResponseDto()
            {
                IsSuccess = true
            };
        }

        /// <summary>
        /// Выполняет поиск плейлистов по названию с пагинацией
        /// </summary>
        /// <param name="dto">Параметры поиска</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Результаты поиска плейлистов</returns>
        public async Task<SearchResponseDto> SearchAsync(SearchRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var playlistsQuery = dbContext.Playlists
                .AsNoTracking()
                .Where(p => EF.Functions.Like(p.Title, $"%{dto.Pattern}%"))
                .Skip((dto.PageNum - 1) * dto.PageSize)
                .Take(dto.PageSize);
            
            var playlists = await playlistsQuery.ToListAsync(ct);
            
            // Load author information for each playlist
            var authorIds = playlists.Select(p => p.AuthorId).Distinct().ToList();
            var authors = new Dictionary<Guid, User>();
            
            foreach (var authorId in authorIds)
            {
                try
                {
                    var response = await _userService.GetByIdAsync(new GetByIdRequest { Id = authorId.ToString() }, cancellationToken: ct);
                    if (response?.User != null)
                    {
                        authors[authorId] = _mapper.Map<User>(response.User);
                    }
                }
                catch (Exception)
                {
                    // Continue if user not found
                }
            }
            
            // Set author for each playlist
            foreach (var playlist in playlists)
            {
                if (authors.TryGetValue(playlist.AuthorId, out var author))
                {
                    playlist.Author = author;
                }
            }

            var playlistDtos = playlists.Select(p => _mapper.Map<PlaylistDto>(p)).ToList();

            return new SearchResponseDto()
            {
                Pattern = dto.Pattern,
                PageNum = dto.PageNum,
                PageSize = dto.PageSize,
                Playlists = playlistDtos
            };
        }

        /// <summary>
        /// Получает список последних созданных плейлистов
        /// </summary>
        /// <param name="dto">Параметры запроса с количеством записей</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список последних созданных плейлистов</returns>
        public async Task<GetLatestPlaylistsResponseDto> GetLatestPlaylistsAsync(GetLatestPlaylistsRequestDto dto, CancellationToken ct = default)
        {
            int count = dto.Count > 0 ? dto.Count : 10; // По умолчанию 10, если передано некорректное значение
            
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);
            
            var latestPlaylists = await dbContext.Playlists
                .AsNoTracking()
                .Include(p => p.Tracks)
                .ThenInclude(pt => pt.Track)
                .Where(p => !p.IsFavorite) // Исключаем плейлисты-избранное, так как они создаются автоматически
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync(ct);

            // Загрузим информацию об авторах для каждого плейлиста
            var authorIds = latestPlaylists.Select(p => p.AuthorId).Distinct().ToList();
            var authors = new Dictionary<Guid, User>();
            
            foreach (var authorId in authorIds)
            {
                try
                {
                    var userResponse = await _userService.GetByIdAsync(
                        new GetByIdRequest { Id = authorId.ToString() }, 
                        cancellationToken: ct);
                        
                    if (userResponse?.User != null)
                    {
                        authors[authorId] = _mapper.Map<User>(userResponse.User);
                    }
                }
                catch (Exception ex)
                {
                    // Продолжаем работу даже если не удалось получить информацию об авторе
                }
            }
            
            // Присвоим авторов плейлистам
            foreach (var playlist in latestPlaylists)
            {
                if (authors.TryGetValue(playlist.AuthorId, out var author))
                {
                    playlist.Author = author;
                }
            }

            var playlistDtos = latestPlaylists.Select(playlist => _mapper.Map<PlaylistDto>(playlist)).ToList();

            return new GetLatestPlaylistsResponseDto
            {
                Playlists = playlistDtos
            };
        }
    }
}
