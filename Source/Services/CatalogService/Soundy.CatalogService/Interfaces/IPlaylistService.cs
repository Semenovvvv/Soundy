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

        /// <summary>
        /// Выполняет поиск плейлистов по названию с пагинацией
        /// </summary>
        /// <param name="dto">Параметры поиска</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Результаты поиска плейлистов</returns>
        Task<SearchResponseDto> SearchAsync(SearchRequestDto dto, CancellationToken ct = default);

        /// <summary>
        /// Получает список последних созданных плейлистов
        /// </summary>
        /// <param name="dto">Параметры запроса с количеством записей</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список последних созданных плейлистов</returns>
        Task<GetLatestPlaylistsResponseDto> GetLatestPlaylistsAsync(GetLatestPlaylistsRequestDto dto, CancellationToken ct = default);
    }
}
