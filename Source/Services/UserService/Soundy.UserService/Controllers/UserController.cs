using AutoMapper;
using Grpc.Core;
using Soundy.UserService.Dto;
using Soundy.UserService.Interfaces;

namespace Soundy.UserService.Controllers
{
    public class UserGrpcController : UserGrpcService.UserGrpcServiceBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserGrpcController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<CreateUserRequestDto>(request);
            var response = await _userService.CreateUserAsync(dto, context.CancellationToken);
            return _mapper.Map<CreateUserResponse>(response);
        }

        public override async Task<GetUserByIdResponse> GetUserById(GetUserByIdRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<GetUserByIdRequestDto>(request);
            var response = await _userService.GetUserById(dto);
            return _mapper.Map<GetUserByIdResponse>(response);
        }

        public override async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<UpdateUserRequestDto>(request);
            var response = await _userService.UpdateUser(dto);
            return _mapper.Map<UpdateUserResponse>(response);
        }

        public override async Task<DeleteUserResponse> DeleteUser(DeleteUserRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<DeleteUserRequestDto>(request);
            var response = await _userService.DeleteUser(dto);
            return _mapper.Map<DeleteUserResponse>(response);
        }

        public override async Task<SearchUsersResponse> SearchUsers(SearchUsersRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<SearchUsersRequestDto>(request);
            var response = await _userService.SearchUsers(dto);
            return _mapper.Map<SearchUsersResponse>(response);
        }
    }
}
