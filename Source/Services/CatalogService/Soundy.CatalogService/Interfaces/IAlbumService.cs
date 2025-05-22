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
    }
}
