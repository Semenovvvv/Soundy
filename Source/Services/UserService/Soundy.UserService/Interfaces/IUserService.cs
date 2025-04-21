using Soundy.UserService.Dto;

namespace Soundy.UserService.Interfaces
{
    public interface IUserService
    {
        public Task<CreateUserResponseDto> CreateUserAsync(CreateUserRequestDto dto, CancellationToken ct = default);

        public Task<GetUserByIdResponseDto> GetUserById(GetUserByIdRequestDto dto, CancellationToken ct = default);

        public Task<UpdateUserResponseDto> UpdateUser(UpdateUserRequestDto dto, CancellationToken ct = default);

        public Task<DeleteUserResponseDto> DeleteUser(DeleteUserRequestDto dto, CancellationToken ct = default);

        public Task<SearchUsersResponseDto> SearchUsers(SearchUsersRequestDto dto, CancellationToken ct = default);
    }
}
