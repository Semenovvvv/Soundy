using AutoMapper;
using Grpc.Core;
using Soundy.SharedLibrary.Contracts.User;
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

        public override async Task<CreateResponse> Create(CreateRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<CreateRequestDto>(request);
            var response = await _userService.CreateUserAsync(dto, context.CancellationToken);
            return _mapper.Map<CreateResponse>(response);
        }

        public override async Task<GetByIdResponse> GetById(GetByIdRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<GetByIdRequestDto>(request);
            var response = await _userService.GetUserById(dto);
            return _mapper.Map<GetByIdResponse>(response);
        }

        public override async Task<UpdateResponse> Update(UpdateRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<UpdateRequestDto>(request);
            var response = await _userService.UpdateUser(dto);
            return _mapper.Map<UpdateResponse>(response);
        }

        public override async Task<DeleteResponse> Delete(DeleteRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<DeleteRequestDto>(request);
            var response = await _userService.DeleteUser(dto);
            return _mapper.Map<DeleteResponse>(response);
        }

        public override async Task<SearchResponse> Search(SearchRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<SearchRequestDto>(request);
            var response = await _userService.SearchUsers(dto);
            return _mapper.Map<SearchResponse>(response);
        }
    }
}
