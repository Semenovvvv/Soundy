using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Soundy.UserService.Controllers;
using Soundy.UserService.DataAccess;
using Soundy.UserService.Extensions;
using Soundy.UserService.Services;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration.AddEnvironmentVariables();

    var configuration = builder.Configuration;

    builder.Services.AddGrpc();
    builder.Services.AddScoped<UserService>();

    builder.Services.ConfigureContext(configuration);

    builder.WebHost.ConfigureKestrel(options =>
    {
        var hostPort = configuration.GetValue<int>("USER_SERVICE_KESTREL_PORT", 5005);
        options.ListenAnyIP(hostPort, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
    });

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<DatabaseContext>();

        try
        {
            context.Database.Migrate();
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

