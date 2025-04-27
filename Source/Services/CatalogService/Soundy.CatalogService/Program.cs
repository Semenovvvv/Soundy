using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Soundy.CatalogService.Controllers;
using Soundy.CatalogService.DataAccess;
using Soundy.CatalogService.Extensions;
using Soundy.CatalogService.Interfaces;
using Soundy.CatalogService.Mappers;
using Soundy.CatalogService.Services;
using Soundy.CatalogService.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var configuration = builder.Configuration;

builder.Services.AddGrpc();

builder.Services.AddAutoMapper(typeof(PlaylistMapper));
builder.Services.AddAutoMapper(typeof(TrackMapper));
builder.Services.AddScoped<ITrackService, TrackService>();
builder.Services.AddScoped<IPlaylistService, PlaylistService>();
builder.Services.AddUserServiceClient(configuration);

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

app.MapGrpcService<TrackGrpcController>();
app.MapGrpcService<PlaylistGrpcController>();

app.Run();
 