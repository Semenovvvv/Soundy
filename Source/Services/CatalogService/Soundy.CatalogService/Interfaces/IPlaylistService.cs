using Soundy.CatalogService.Dto.PlaylistDtos;

namespace Soundy.CatalogService.Interfaces
{
    public interface IPlaylistService
    {
        Task<CreateResponseDto> CreateAsync(CreateRequestDto dto, CancellationToken ct = default);

        Task<CreateFavoriteResponseDto> CreateFavoriteAsync(CreateFavoriteRequestDto dto, CancellationToken ct = default);

        Task<GetByIdResponseDto> GetByIdAsync(GetByIdRequestDto dto, CancellationToken ct = default);

        Task<GetListByAuthorIdResponseDto> GetListByAuthorIdAsync(GetListByAuthorIdRequestDto dto, CancellationToken ct = default);

        Task<GetFavoriteResponseDto> GetFavoriteAsync(GetFavoriteRequestDto dto, CancellationToken ct = default);

        Task<AddTrackResponseDto> AddTrackAsync(AddTrackRequestDto dto, CancellationToken ct = default);

        Task<UpdateResponseDto> UpdateAsync(UpdateRequestDto dto, CancellationToken ct = default);

        Task<DeleteResponseDto> DeleteAsync(DeleteRequestDto dto, CancellationToken ct = default);
    }
}
