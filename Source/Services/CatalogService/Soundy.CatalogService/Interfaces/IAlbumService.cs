using Soundy.CatalogService.Dto.AlbumDtos;

namespace Soundy.CatalogService.Interfaces
{
    public interface IAlbumService
    {
        /// <summary>
        /// Создает новый альбом и загружает информацию об авторе
        /// </summary>
        /// <param name="dto">Данные для создания альбома</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Информация о созданном альбоме</returns>
        Task<CreateResponseDto> CreateAsync(CreateRequestDto dto, CancellationToken ct = default);

        /// <summary>
        /// Добавляет трек в альбом и обновляет информацию об авторе
        /// </summary>
        /// <param name="dto">Данные трека и альбома</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Обновленная информация об альбоме</returns>
        Task<AddTrackResponseDto> AddTrackAsync(AddTrackRequestDto dto, CancellationToken ct = default);

        /// <summary>
        /// Получает информацию об альбоме по его идентификатору
        /// </summary>
        /// <param name="dto">Идентификатор альбома</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Детальная информация об альбоме с данными автора</returns>
        Task<GetByIdResponseDto> GetByIdAsync(GetByIdRequestDto dto, CancellationToken ct = default);

        /// <summary>
        /// Получает список альбомов по идентификатору автора
        /// </summary>
        /// <param name="dto">Идентификатор автора</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список альбомов автора</returns>
        Task<GetByAuthorIdResponseDto> GetByAuthorIdAsync(GetByAuthorIdRequestDto dto, CancellationToken ct = default);

        /// <summary>
        /// Выполняет поиск альбомов по названию с пагинацией
        /// </summary>
        /// <param name="dto">Параметры поиска</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Результаты поиска альбомов</returns>
        Task<SearchResponseDto> SearchAsync(SearchRequestDto dto, CancellationToken ct = default);

        /// <summary>
        /// Получает список последних созданных альбомов
        /// </summary>
        /// <param name="dto">Параметры запроса с количеством записей</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список последних созданных альбомов</returns>
        Task<GetLatestAlbumsResponseDto> GetLatestAlbumsAsync(GetLatestAlbumsRequestDto dto, CancellationToken ct = default);
    }
}
