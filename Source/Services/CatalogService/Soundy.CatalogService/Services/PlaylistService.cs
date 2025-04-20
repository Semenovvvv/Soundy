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

        public async Task<CreatePlaylistResponseDto> CreatePlaylistAsync(CreatePlaylistRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var playlist = new Playlist()
            {
                Name = dto.Name,
                AuthorId = dto.AuthorId
            };

            await dbContext.Playlists.AddAsync(playlist, ct);
            await dbContext.SaveChangesAsync(ct);

            var response = new CreatePlaylistResponseDto()
            {
                Id = playlist.Id,
                AuthorId = playlist.AuthorId,
                Name = playlist.Name
            };

            return response;
        }
    }
}
