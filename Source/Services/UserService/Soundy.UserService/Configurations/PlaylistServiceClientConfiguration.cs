using Service.Playlist;

namespace Soundy.UserService.Configurations
{
    public static class PlaylistServiceClientConfiguration
    {
        public static IServiceCollection AddPlaylistServiceClient(this IServiceCollection services, IConfiguration configuration)
        {
            const string catalogServiceUri = "CATALOG_SERVICE_URI";

            services.AddGrpcClient<PlaylistGrpcService.PlaylistGrpcServiceClient>(options =>
                options.Address = new Uri(configuration[catalogServiceUri] ?? throw new ArgumentException(catalogServiceUri)));

            return services;
        }
    }
}
