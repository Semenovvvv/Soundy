using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Soundy.UserService.Configurations;
using Soundy.UserService.Controllers;
using Soundy.UserService.DataAccess;
using Soundy.UserService.Extensions;
using Soundy.UserService.Interfaces;
using Soundy.UserService.Mappers;
using Soundy.UserService.Services;

try
{
     var builder = WebApplication.CreateBuilder(args);

    builder.Configuration.AddEnvironmentVariables();

    var configuration = builder.Configuration;

    builder.Services.AddGrpc();
    
    // Добавляем сжатие ответов
    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
        options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();
    });

    builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
    {
        options.Level = System.IO.Compression.CompressionLevel.Fastest;
    });

    builder.Services.Configure<GzipCompressionProviderOptions>(options =>
    {
        options.Level = System.IO.Compression.CompressionLevel.Fastest;
    });

    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddAutoMapper(typeof(UserServiceMapper));
    builder.Services.ConfigureContext(configuration);
    
    // Явно указываем, какой метод расширения использовать
    Soundy.UserService.Configurations.PlaylistServiceClientConfiguration.AddPlaylistServiceClient(builder.Services, configuration);
    Soundy.UserService.Extensions.GrpcClientExtensions.AddIAMServiceClient(builder.Services, configuration);

    builder.WebHost.ConfigureKestrel(options =>
    {
        var hostPort = configuration.GetValue("USER_SERVICE_PORT", 5005);
        options.ListenAnyIP(hostPort, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
    });

    var app = builder.Build();

    // Включаем сжатие ответов
    app.UseResponseCompression();

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = await services.GetRequiredService<IDbContextFactory<DatabaseContext>>().CreateDbContextAsync();
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Migrate error: {ex.Message}");
        }
    }

    app.MapGrpcService<UserGrpcController>();

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Error start user service: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    Console.WriteLine($"Inner exception stack trace: {ex.InnerException?.StackTrace}");
}

