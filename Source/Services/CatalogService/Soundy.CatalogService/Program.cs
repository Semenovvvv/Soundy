using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.ResponseCompression;
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

builder.Services.AddAutoMapper(typeof(PlaylistMapper));
builder.Services.AddAutoMapper(typeof(TrackMapper));
builder.Services.AddAutoMapper(typeof(AlbumMapper));
builder.Services.AddAutoMapper(typeof(DtoMapper));
builder.Services.AddScoped<ITrackService, TrackService>();
builder.Services.AddScoped<IPlaylistService, PlaylistService>();
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddUserServiceClient(configuration);

builder.Services.ConfigureContext(configuration);

builder.WebHost.ConfigureKestrel(options =>
{
    var hostPort = configuration.GetValue("CATALOG_SERVICE_PORT", 5006);
    options.ListenAnyIP(hostPort, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
});

var app = builder.Build();

// Включаем сжатие ответов
app.UseResponseCompression();

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
app.MapGrpcService<AlbumGrpcController>();

app.Run();
 