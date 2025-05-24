using Grpc.Net.Client;
using Service.Iam;
using Service.Playlist;

namespace Soundy.UserService.Extensions
{
    public static class GrpcClientExtensions
    {
        // Метод закомментирован, так как есть конфликт с существующим методом
        /*
        public static IServiceCollection AddPlaylistServiceClient(this IServiceCollection services, IConfiguration configuration)
        {
            var playlistServiceUrl = configuration["PLAYLIST_SERVICE_URL"] ?? "http://localhost:5004";
            services.AddGrpcClient<PlaylistGrpcService.PlaylistGrpcServiceClient>(options =>
            {
                options.Address = new Uri(playlistServiceUrl);
            });

            return services;
        }
        */

        public static IServiceCollection AddIAMServiceClient(this IServiceCollection services, IConfiguration configuration)
        {
            var iamServiceUrl = configuration["IAM_SERVICE_URL"] ?? "http://localhost:5003";
            services.AddGrpcClient<IAMGrpcService.IAMGrpcServiceClient>(options =>
            {
                options.Address = new Uri(iamServiceUrl);
            });

            return services;
        }
    }
} 