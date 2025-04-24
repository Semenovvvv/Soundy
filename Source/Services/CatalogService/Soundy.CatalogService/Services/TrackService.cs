using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Soundy.CatalogService.DataAccess;
using Soundy.CatalogService.Dto.TrackDto;
using Soundy.CatalogService.Entities;
using Soundy.CatalogService.Interfaces;

namespace Soundy.CatalogService.Services
{
    public class TrackService : ITrackService
    {
        private readonly IDbContextFactory<DatabaseContext> _dbFactory;
        private readonly ILogger<TrackService> _logger;

        public TrackService(IDbContextFactory<DatabaseContext> dbFactory, ILogger<TrackService> logger)
        {
            _dbFactory = dbFactory;
            _logger = logger;
        }

        public async Task<CreateResponseDto> CreateAsync(CreateRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var track = new Track()
            {
                PlaylistId = dto.PlaylistId,
                UserId = dto.UserId,
                Title = dto.Title,
                Duration = dto.Duration,
                CreatedAt = DateTime.UtcNow
            };

            await dbContext.Tracks.AddAsync(track, ct);
            await dbContext.SaveChangesAsync(ct);

            return new CreateResponseDto
            {
                Track = new TrackDto()
                {
                    Id = track.Id,
                    PlaylistId = track.PlaylistId,
                    UserId = track.UserId,
                    Title = track.Title,
                    Duration = track.Duration,
                    CreatedAt = track.CreatedAt
                }
            };
        }

        public async Task<GetByIdResponseDto> GetByIdAsync(GetByIdRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var track = await dbContext.Tracks.FirstOrDefaultAsync(x => x.Id == dto.Id, ct);

            if (track is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Track with Id {dto.Id} not found"));

            return new GetByIdResponseDto
            {
                Track = new TrackDto()
                {
                    Id = track.Id,
                    PlaylistId = track.PlaylistId,
                    UserId = track.UserId,
                    Title = track.Title,
                    Duration = track.Duration,
                    CreatedAt = track.CreatedAt
                }
            };
        }

        public async Task<GetListByPlaylistResponseDto> GetListByPlaylistAsync(GetListByPlaylistRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var playlist = await dbContext.Playlists.Include(playlist => playlist.Tracks).FirstOrDefaultAsync(x => x.Id == dto.PlaylistId, ct);

            if (playlist is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Playlist with Id {dto.PlaylistId} not found"));

            var tracks = playlist.Tracks.Select(x => new TrackDto()
            {
                Id = x.Id,
                PlaylistId = x.PlaylistId,
                UserId = x.UserId,
                Title = x.Title,
                Duration = x.Duration,
                CreatedAt = x.CreatedAt
            }).ToList();

            return new GetListByPlaylistResponseDto()
            {
                PlaylistId = dto.PlaylistId,
                Playlist = new PlaylistDto()
                {
                    Id = playlist.Id,
                    AuthorId = playlist.AuthorId,
                    Name = playlist.Name,
                    CreatedAt = playlist.CreatedAt
                },
                Tracks = tracks
            };
        }

        public async Task<SearchResponseDto> SearchAsync(SearchRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var tracks = await dbContext.Tracks
                .Where(t => EF.Functions.Like(t.Title, $"%{dto.Pattern}%"))
                .Skip((dto.PageNum - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select(x => new TrackDto()
                {
                    Id = x.Id,
                    PlaylistId = x.PlaylistId,
                    UserId = x.UserId,
                    Title = x.Title,
                    Duration = x.Duration,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync(ct);

            return new SearchResponseDto()
            {
                Pattern = dto.Pattern,
                PageNum = dto.PageNum,
                PageSize = dto.PageSize,
                Tracks = tracks
            };
        }

        public async Task<UpdateResponseDto> UpdateAsync(UpdateRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var track = await dbContext.Tracks.FirstOrDefaultAsync(x => x.Id == dto.Id, ct);

            if (track is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Track with Id {dto.Id} not found"));

            track.Title = dto.Title;

            await dbContext.SaveChangesAsync(ct);
            return new UpdateResponseDto()
            {
                Track = new TrackDto()
                {
                    Id = track.Id,
                    PlaylistId = track.PlaylistId,
                    UserId = track.UserId,
                    Title = track.Title,
                    Duration = track.Duration,
                    CreatedAt = track.CreatedAt
                }
            };
        }

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
    }
}
