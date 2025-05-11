using Soundy.CatalogService.Dto.TrackDtos;

namespace Soundy.CatalogService.Interfaces
{
    public interface ITrackService
    {
        public Task<CreateResponseDto> CreateAsync(CreateRequestDto dto, CancellationToken ct = default);

        public Task<GetByIdResponseDto> GetByIdAsync(GetByIdRequestDto dto, CancellationToken ct = default);

        public Task<GetListByPlaylistResponseDto> GetListByPlaylistAsync(GetListByPlaylistRequestDto dto, CancellationToken ct = default);

        public Task<SearchResponseDto> SearchAsync(SearchRequestDto dto, CancellationToken ct = default);

        public Task<UpdateResponseDto> UpdateAsync(UpdateRequestDto dto, CancellationToken ct = default);

        public Task<DeleteResponseDto> DeleteAsync(DeleteRequestDto dto, CancellationToken ct = default);

        public Task<GetListByUserIdResponseDto> GetListByUserIdRequest(GetListByUserIdRequestDto dto, CancellationToken ct = default);
    }
}
