using Soundy.CatalogService.Dto.PlaylistDto;

namespace Soundy.CatalogService.Interfaces
{
    public interface IPlaylistService
    {
        Task<CreatePlaylistResponseDto> CreatePlaylistAsync(CreatePlaylistRequestDto dto, CancellationToken ct = default);
    }
}
