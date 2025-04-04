using Grpc.Core;
using Soundy.SharedLibrary.Enums;
using Soundy.UserService.Dto;

namespace Soundy.UserService.Controllers
{
    public class UserGrpcController : UserGrpcService.UserGrpcServiceBase
    {
        private readonly Services.UserService _userService;
        private readonly ILogger<UserGrpcController> _logger;

        public UserGrpcController(Services.UserService userService, ILogger<UserGrpcController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public override async Task<GetUserResponse> GetUserById(GetUserByIdRequest request, ServerCallContext context)
        {
            try
            {
                var response = await _userService.GetUserById(Guid.Parse(request.UserId), context.CancellationToken);

                if (response is { Status: ResponseStatus.Success, Data: not null })
                {
                    return new GetUserResponse
                    {
                        User = new UserData()
                        {
                            Id = response.Data.Id.ToString(),
                            Login = response.Data.Login,
                            Email = response.Data.Email
                        }
                    };
                }

                return new GetUserResponse
                {
                    Error = new OperationResponse
                    {
                        Status = MapStatus(response.Status),
                        Message = response.Message ?? string.Empty
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetUserById");
                return new GetUserResponse
                {
                    Error = new OperationResponse
                    {
                        Status = OperationResponse.Types.Status.Failed,
                        Message = "Internal server error"
                    }
                };
            }
        }

        public override async Task<OperationResponse> UpdateUserProfile(UpdateUserProfileRequest request, ServerCallContext context)
        {
            try
            {
                var dto = new UpdateUserDto
                {
                    Username = request.Username,
                    Email = request.Email
                };

                var response = await _userService.UpdateUserProfile(Guid.Parse(request.UserId), dto, context.CancellationToken);

                return new OperationResponse
                {
                    Status = MapStatus(response.Status),
                    Message = response.Message ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateUserProfile");
                return new OperationResponse
                {
                    Status = OperationResponse.Types.Status.Failed,
                    Message = "Internal server error"
                };
            }
        }

        public override async Task<OperationResponse> DeleteUser(DeleteUserRequest request, ServerCallContext context)
        {
            try
            {
                var response = await _userService.DeleteUser(Guid.Parse((ReadOnlySpan<char>)request.UserId), context.CancellationToken);

                return new OperationResponse
                {
                    Status = MapStatus(response.Status),
                    Message = response.Message ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteUser");
                return new OperationResponse
                {
                    Status = OperationResponse.Types.Status.Failed,
                    Message = "Internal server error"
                };
            }
        }

        public override async Task<SearchUsersResponse> SearchUsers(SearchUsersRequest request, ServerCallContext context)
        {
            try
            {
                var response = await _userService.GetUsersByName(
                    request.SearchPattern,
                    request.PageNumber,
                    request.PageSize, 
                    context.CancellationToken);

                if (response.Status != ResponseStatus.Success)
                {
                    return new SearchUsersResponse
                    {
                        Status = MapStatus(response.Status),
                        Message = response.Message ?? string.Empty
                    };
                }

                var result = new SearchUsersResponse
                {
                    Pagination = new PaginationInfo
                    {
                        TotalCount = response.TotalCount,
                        PageNumber = response.PageNumber,
                        PageSize = response.PageSize
                    },
                    Status = OperationResponse.Types.Status.Success
                };

                result.Users.AddRange(response.Items.Select(user => new UserData
                {
                    Id = user.Id.ToString(),
                    Login = user.Login,
                    Email = user.Email
                }));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SearchUsers");
                return new SearchUsersResponse
                {
                    Status = OperationResponse.Types.Status.Failed,
                    Message = "Internal server error"
                };
            }
        }

        private static OperationResponse.Types.Status MapStatus(ResponseStatus status)
        {
            return status switch
            {
                ResponseStatus.Success => OperationResponse.Types.Status.Success,
                ResponseStatus.NotFound => OperationResponse.Types.Status.NotFound,
                ResponseStatus.InvalidInput => OperationResponse.Types.Status.InvalidInput,
                ResponseStatus.Conflict => OperationResponse.Types.Status.Conflict,
                _ => OperationResponse.Types.Status.Failed,
            };
        }
    }
}
