using Soundy.CatalogService.Dto.AlbumDtos;

namespace Soundy.CatalogService.Interfaces
{
    public interface IAlbumService
    {
        Task<CreateResponseDto> CreateAsync(CreateRequestDto dto, CancellationToken ct = default);

        Task<AddTrackResponseDto> AddTrackAsync(AddTrackRequestDto dto, CancellationToken ct = default);
    }
}
