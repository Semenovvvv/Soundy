using Soundy.UserService.Dto;

namespace Soundy.UserService.Interfaces
{
    public interface IUserService
    {
        public Task<CreateResponseDto> CreateUserAsync(CreateRequestDto dto, CancellationToken ct = default);

        public Task<GetByIdResponseDto> GetUserById(GetByIdRequestDto dto, CancellationToken ct = default);

        public Task<UpdateResponseDto> UpdateUser(UpdateRequestDto dto, CancellationToken ct = default);

        public Task<DeleteResponseDto> DeleteUser(DeleteRequestDto dto, CancellationToken ct = default);

        public Task<SearchResponseDto> SearchUsers(SearchRequestDto dto, CancellationToken ct = default);

        /// <summary>
        /// Получает список последних зарегистрированных пользователей
        /// </summary>
        /// <param name="dto">Параметры запроса с количеством записей</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список последних зарегистрированных пользователей</returns>
        public Task<GetLatestUsersResponseDto> GetLatestUsersAsync(GetLatestUsersRequestDto dto, CancellationToken ct = default);
    }
}
