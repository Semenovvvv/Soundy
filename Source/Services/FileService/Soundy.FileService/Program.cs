using Microsoft.AspNetCore.Server.Kestrel.Core;
using Soundy.FileService.Configurations;
using Soundy.FileService.Controllers;
using Soundy.FileService.Interfaces;
using Soundy.FileService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

var configuration = builder.Configuration;

builder.Services.AddGrpc();

builder.Services.AddScoped<ITrackFileService, MinioFileService>();
builder.Services.ConfigureS3(builder.Configuration);

builder.WebHost.ConfigureKestrel(options =>
{
    var hostPort = configuration.GetValue("CATALOG_SERVICE_PORT", 5007);
    options.ListenAnyIP(hostPort, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
});

var app = builder.Build();

app.MapGrpcService<FileGrpcController>();

app.Run();
