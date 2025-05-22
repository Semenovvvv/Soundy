using Service.File;

namespace Soundy.ApiGateway.Configurations.Services
{
    public static class FileServiceClientConfguration
    {
        private const string fileServiceUriEnv = "FILE_SERVICE_URI";
        public static IServiceCollection AddFileServiceClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddGrpcClient<FileGrpcService.FileGrpcServiceClient>(options =>
                options.Address = new Uri(configuration[fileServiceUriEnv] ?? throw new ArgumentException(fileServiceUriEnv)));

            return services;
        }
    }
}
