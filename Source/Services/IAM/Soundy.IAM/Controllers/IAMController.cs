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

        public async Task<SignInResponse> RefreshToken(string refreshToken)
        {
            var result = await _authService.RefreshTokenAsync(refreshToken);
            
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
    }
}
