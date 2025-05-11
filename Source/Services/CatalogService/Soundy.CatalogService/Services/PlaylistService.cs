using AutoMapper;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Soundy.CatalogService.DataAccess;
using Soundy.CatalogService.Dto;
using Soundy.CatalogService.Dto.PlaylistDtos;
using Soundy.CatalogService.Entities;
using Soundy.CatalogService.Interfaces;
using Soundy.SharedLibrary.Contracts.User;

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
                };

                await dbContext.Playlists.AddAsync(playlist, ct);
                await dbContext.SaveChangesAsync(ct);
                return new CreateFavoriteResponseDto()
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

            return new GetFavoriteResponseDto()
            {
                Playlist = _mapper.Map<PlaylistDto>(playlist)
            };
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
    }
}
