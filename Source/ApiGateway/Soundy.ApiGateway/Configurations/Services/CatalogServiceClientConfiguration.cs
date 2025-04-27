using Soundy.SharedLibrary.Contracts.Playlist;
using Soundy.SharedLibrary.Contracts.Track;

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
    }
}
