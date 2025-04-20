using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Soundy.CatalogService.Configurations;
using Soundy.CatalogService.Controllers;
using Soundy.CatalogService.DataAccess;
using Soundy.CatalogService.Extensions;
using Soundy.CatalogService.Interfaces;
using Soundy.CatalogService.Mappers;
using Soundy.CatalogService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var configuration = builder.Configuration;

builder.Services.AddGrpc();
builder.Services.ConfigureS3(builder.Configuration);
builder.Services.AddAutoMapper(typeof(TrackProfile));
builder.Services.AddScoped<ITrackMetadataService, TrackService>();
builder.Services.AddScoped<ITrackFileService, MinioFileService>();

builder.Services.ConfigureContext(configuration);

builder.WebHost.ConfigureKestrel(options =>
{
    var hostPort = configuration.GetValue("CATALOG_SERVICE_KESTREL_PORT", 5006);
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

app.MapGrpcService<TrackGrpcController>();

app.Run();
 