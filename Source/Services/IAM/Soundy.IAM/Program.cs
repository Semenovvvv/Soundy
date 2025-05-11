using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Soundy.IAM.DataAccess;
using Soundy.IAM.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var configuration = builder.Configuration;

builder.Services.AddScoped<IIAMService, IAMService>();

builder.Services.ConfigureContext(configuration);

builder.WebHost.ConfigureKestrel(options =>
{
    var hostPort = configuration.GetValue("CATALOG_SERVICE_PORT", 5006);
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

app.MapGrpcService<IAMGrpcController>();

app.Run();