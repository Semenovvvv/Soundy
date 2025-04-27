using Soundy.SharedLibrary.Contracts.User;

namespace Soundy.CatalogService.Configurations
{
    public static class UserServiceClientConfiguration
    {
        public static IServiceCollection AddUserServiceClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddGrpcClient<UserGrpcService.UserGrpcServiceClient>(options =>
            {
                var address = new Uri(configuration["USER_SERVICE_URI"] ??
                                      throw new Exception("USER_SERVICE_URI is not specified in ENV"));
                options.Address = address;
            });

            return services;
        }
    }
}
