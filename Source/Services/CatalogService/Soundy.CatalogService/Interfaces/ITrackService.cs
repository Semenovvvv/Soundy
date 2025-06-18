using Soundy.CatalogService.Dto.TrackDtos;

namespace Soundy.CatalogService.Interfaces
{
    public interface ITrackService
    {
        /// <summary>
        /// Создает новый трек и загружает информацию об авторе
        /// </summary>
        /// <param name="dto">Данные для создания трека</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Информация о созданном треке</returns>
        public Task<CreateResponseDto> CreateAsync(CreateRequestDto dto, CancellationToken ct = default);

        /// <summary>
        /// Получает трек по идентификатору с информацией об авторе
        /// </summary>
        /// <param name="dto">Запрос с идентификатором трека</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Информация о треке с данными автора</returns>
        public Task<GetByIdResponseDto> GetByIdAsync(GetByIdRequestDto dto, CancellationToken ct = default);

        /// <summary>
        /// Получает список треков по идентификатору плейлиста
        /// </summary>
        /// <param name="dto">Запрос с идентификатором плейлиста</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список треков плейлиста</returns>
        public Task<GetListByPlaylistResponseDto> GetListByPlaylistAsync(GetListByPlaylistRequestDto dto, CancellationToken ct = default);

        /// <summary>
        /// Выполняет поиск треков по строке запроса с загрузкой информации об авторах
        /// </summary>
        /// <param name="dto">Параметры поиска</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Результаты поиска с информацией об авторах</returns>
        public Task<SearchResponseDto> SearchAsync(SearchRequestDto dto, CancellationToken ct = default);

        /// <summary>
        /// Обновляет информацию о треке и загружает данные автора
        /// </summary>
        /// <param name="dto">Данные для обновления</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Обновленная информация о треке</returns>
        public Task<UpdateResponseDto> UpdateAsync(UpdateRequestDto dto, CancellationToken ct = default);

        /// <summary>
        /// Удаляет трек из системы
        /// </summary>
        /// <param name="dto">Идентификатор удаляемого трека</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Результат операции удаления</returns>
        public Task<DeleteResponseDto> DeleteAsync(DeleteRequestDto dto, CancellationToken ct = default);

        /// <summary>
        /// Получает список треков по идентификатору пользователя
        /// </summary>
        /// <param name="dto">Идентификатор пользователя</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список треков пользователя</returns>
        public Task<GetListByUserIdResponseDto> GetListByUserId(GetListByUserIdRequestDto dto, CancellationToken ct = default);

        /// <summary>
        /// Добавляет лайк треку от пользователя
        /// </summary>
        /// <param name="dto">Данные трека и пользователя</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Результат операции добавления лайка</returns>
        public Task<LikeTrackResponseDto> LikeTrackAsync(LikeTrackRequestDto dto, CancellationToken ct = default);

        /// <summary>
        /// Удаляет лайк трека от пользователя
        /// </summary>
        /// <param name="dto">Данные трека и пользователя</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Результат операции удаления лайка</returns>
        public Task<UnlikeTrackResponseDto> UnlikeTrackAsync(UnlikeTrackRequestDto dto, CancellationToken ct = default);

        /// <summary>
        /// Получает список лайкнутых треков пользователя
        /// </summary>
        /// <param name="dto">Идентификатор пользователя</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список лайкнутых треков</returns>
        public Task<GetLikedTracksResponseDto> GetLikedTracksAsync(GetLikedTracksRequestDto dto, CancellationToken ct = default);
    }
}
