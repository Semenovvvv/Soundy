using Service.Iam;

namespace Soundy.ApiGateway.Configurations.Services;

public static class IamServiceClientConfig
{
    private const string iamServiceUriEnv = "IAM_SERVICE_URI";
    public static IServiceCollection AddIamServiceClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpcClient<IAMGrpcService.IAMGrpcServiceClient>(options =>
            options.Address = new Uri(configuration[iamServiceUriEnv] ?? throw new ArgumentException(iamServiceUriEnv)));

        return services;
    }
} 