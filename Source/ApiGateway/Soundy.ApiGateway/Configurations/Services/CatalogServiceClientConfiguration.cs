using Service.Album;
using Service.Playlist;
using Service.Track;

namespace Soundy.ApiGateway.Configurations.Services
{
    public static class CatalogServiceClientConfiguration
    {
        private const string catalogServiceUriEnv = "CATALOG_SERVICE_URI";
        public static IServiceCollection AddPlaylistServiceClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddGrpcClient<PlaylistGrpcService.PlaylistGrpcServiceClient>(options =>
                options.Address = new Uri(configuration[catalogServiceUriEnv] ?? throw new ArgumentException(catalogServiceUriEnv)));

            return services;
        }

        public static IServiceCollection AddTrackServiceClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddGrpcClient<TrackGrpcService.TrackGrpcServiceClient>(options =>
                options.Address = new Uri(configuration[catalogServiceUriEnv] ?? throw new ArgumentException(catalogServiceUriEnv)));

            return services;
        }

        public static IServiceCollection AddAlbumServiceClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddGrpcClient<AlbumGrpcService.AlbumGrpcServiceClient>(options =>
                options.Address = new Uri(configuration[catalogServiceUriEnv] ?? throw new ArgumentException(catalogServiceUriEnv)));

            return services;
        }
    }
}
