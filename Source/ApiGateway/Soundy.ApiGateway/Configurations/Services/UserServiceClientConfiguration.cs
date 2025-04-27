using Soundy.SharedLibrary.Contracts.User;

namespace Soundy.ApiGateway.Configurations.Services
{
    public static class UserServiceClientConfiguration
    {
        private const string userServiceUriEnv = "USER_SERVICE_URI";
        public static IServiceCollection AddUserServiceClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddGrpcClient<UserGrpcService.UserGrpcServiceClient>(options =>
                options.Address = new Uri(configuration[userServiceUriEnv] ?? throw new ArgumentException(userServiceUriEnv)));

            return services;
        }
    }
}
