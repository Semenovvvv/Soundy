using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Soundy.CatalogService.DataAccess;
using Soundy.CatalogService.Dto.PlaylistDto;
using Soundy.CatalogService.Entities;
using Soundy.CatalogService.Interfaces;

namespace Soundy.CatalogService.Services
{
    public class PlaylistService(IDbContextFactory<DatabaseContext> dbFactory) : IPlaylistService
    {
        private IDbContextFactory<DatabaseContext> _dbFactory = dbFactory;

        public async Task<CreateResponseDto> CreateAsync(CreateRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var playlist = new Playlist()
            {
                AuthorId = dto.AuthorId,
                Name = dto.Name,
                CreatedAt = DateTime.UtcNow
            };
            await dbContext.Playlists.AddAsync(playlist, ct);
            await dbContext.SaveChangesAsync(ct);
            return new CreateResponseDto()
            {
                Id = playlist.Id,
                AuthorId = playlist.AuthorId,
                Name = playlist.Name
            };
        }

        public async Task<GetByIdResponseDto> GetByIdAsync(GetByIdRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var playlist = await dbContext.Playlists
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == dto.Id, ct);

            var tracks = await dbContext.Tracks
                .Where(x => x.PlaylistId == dto.Id)
                .Select(x => new TrackDto()
                {
                    TrackId = x.Id,
                    Title = x.Title
                }).ToListAsync(ct);

            if (playlist is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Playlist with Id = {dto.Id} not found"));

            return new GetByIdResponseDto()
            {
                Id = playlist.Id,
                AuthorId = playlist.AuthorId,
                Name = playlist.Name,
                Tracks = tracks
            };
        }

        public async Task<GetListByAuthorIdResponseDto> GetListByAuthorIdAsync(GetListByAuthorIdRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var playlists = await dbContext.Playlists
                .AsNoTracking()
                .Where(x => x.AuthorId == dto.AuthorId)
                .Select(x => new PlaylistDto()
                {
                    Id = x.Id,
                    AuthorId = x.AuthorId,
                    Name = x.Name
                })
                .ToListAsync(ct);

            return new GetListByAuthorIdResponseDto()
            {
                AuthorId = dto.AuthorId,
                Playlists = playlists
            };
        }

        public async Task<UpdateResponseDto> UpdateAsync(UpdateRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var playlist = await dbContext.Playlists.FirstOrDefaultAsync(x => x.Id == dto.Id, ct);

            if (playlist is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Playlist with Id = {dto.Id} not found"));
            }

            playlist.Name = dto.Name;
            await dbContext.SaveChangesAsync(ct);

            return new UpdateResponseDto()
            {
                Id = dto.Id,
                AuthorId = playlist.AuthorId,
                Name = playlist.Name
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
