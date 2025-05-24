using Grpc.Core;
using Service.Iam;
using Soundy.IAM.Interfaces;

namespace Soundy.IAM.Controllers
{
    public class IAMGrpcController : IAMGrpcService.IAMGrpcServiceBase
    {
        private readonly IAuthService _authService;

        public IAMGrpcController(IAuthService authService)
        {
            _authService = authService;
        }

        public override async Task<SignUpResponse> SignUp(SignUpRequest request, ServerCallContext context)
        {
            var result = await _authService.SignUpAsync(request.Username, request.Email, request.Password);
            return new SignUpResponse { Success = result };
        }

        public override async Task<SignInResponse> SignIn(SignInRequest request, ServerCallContext context)
        {
            var result = await _authService.SignInAsync(request.Username, request.Password);
            
            if (!result.Success)
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, result.ErrorMessage));
            }

            return new SignInResponse
            {
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken,
                UserId = result.UserId
            };
        }

        public override async Task<SignOutResponse> SignOut(SignOutRequest request, ServerCallContext context)
        {
            var result = await _authService.SignOutAsync(request.RefreshToken);
            return new SignOutResponse { Success = result };
        }

        public override async Task<ValidateTokenResponse> ValidateToken(ValidateTokenRequest request, ServerCallContext context)
        {
            var result = await _authService.ValidateTokenAsync(request.Token);
            return new ValidateTokenResponse
            {
                IsValid = result.IsValid,
                UserId = result.UserId ?? string.Empty
            };
        }

        public override async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
        {
            var result = await _authService.RefreshTokenAsync(request.RefreshToken);
            
            return new RefreshTokenResponse
            {
                Success = result.Success,
                ErrorMessage = result.ErrorMessage ?? string.Empty,
                AccessToken = result.AccessToken ?? string.Empty,
                RefreshToken = result.RefreshToken ?? string.Empty,
                UserId = result.UserId ?? string.Empty
            };
        }

        public override async Task<UpdateUserDataResponse> UpdateUserData(UpdateUserDataRequest request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.UserId))
            {
                return new UpdateUserDataResponse
                {
                    Success = false,
                    ErrorMessage = "User ID is required"
                };
            }

            if (string.IsNullOrEmpty(request.Username))
            {
                return new UpdateUserDataResponse
                {
                    Success = false,
                    ErrorMessage = "Username is required"
                };
            }

            var result = await _authService.UpdateUserDataAsync(
                request.UserId,
                request.Username,
                request.HasEmail ? request.Email : null);

            return new UpdateUserDataResponse
            {
                Success = result.Success,
                ErrorMessage = result.ErrorMessage ?? string.Empty
            };
        }

        public override async Task<DeleteUserResponse> DeleteUser(DeleteUserRequest request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.UserId))
            {
                return new DeleteUserResponse
                {
                    Success = false,
                    ErrorMessage = "User ID is required"
                };
            }

            var result = await _authService.DeleteUserAsync(request.UserId);

            return new DeleteUserResponse
            {
                Success = result.Success,
                ErrorMessage = result.ErrorMessage ?? string.Empty
            };
        }
    }
}
