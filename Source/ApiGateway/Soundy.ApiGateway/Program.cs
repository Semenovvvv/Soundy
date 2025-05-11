using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;
using Soundy.ApiGateway.Configurations.Services;
using Soundy.ApiGateway.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddGrpc();

builder.Services.AddFileServiceClient(configuration);
builder.Services.AddTrackServiceClient(configuration);
builder.Services.AddPlaylistServiceClient(configuration);
builder.Services.AddUserServiceClient(configuration);
builder.Services.AddAlbumServiceClient(configuration);

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.SetMinimumLevel(LogLevel.Debug);
    loggingBuilder.AddConsole();
});

builder.Services.AddSwaggerGen(options =>
{
    options.SupportNonNullableReferenceTypes();

    options.CustomSchemaIds(type =>
        type.FullName?
            .Replace("Soundy.SharedLibrary.Contracts.", "")
            .Replace(".", "")
            .Replace("+", ".")
    );

    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Soundy API", Version = "v1" });
});

builder.WebHost.ConfigureKestrel(o =>
{
    o.ListenAnyIP(8085, listenOptions => listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3);
});

var app = builder.Build();

app.UseMiddleware<GrpcExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();

app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.MapControllers();

app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(context =>
    {
        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (exceptionHandlerFeature != null)
        {
            if (app.Services.GetService<ILogger>() is { } logger)
            {
                logger.LogError(exceptionHandlerFeature.Error, "UseExceptionHandler поймал ошибку в WebHost");
            }
        }
        return Task.CompletedTask;
    });
});

app.Run();
