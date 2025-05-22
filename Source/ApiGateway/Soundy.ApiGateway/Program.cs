using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Soundy.ApiGateway.Configurations;
using Soundy.ApiGateway.Configurations.Services;
using Soundy.ApiGateway.Middlewares;
using Soundy.ApiGateway.Services;
using Soundy.ApiGateway.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddGrpc();

// Конфигурация JWT
builder.Services.Configure<JwtConfig>(configuration.GetSection("JwtConfig"));
var jwtConfig = configuration.GetSection("JwtConfig").Get<JwtConfig>();

// Добавление аутентификации
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig?.Secret ?? "defaultkey")),
        ValidateIssuer = true,
        ValidIssuer = jwtConfig?.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtConfig?.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
    
    // Настройка для приема токенов как с префиксом Bearer, так и без него
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            
            if (!string.IsNullOrEmpty(authHeader))
            {
                // Если заголовок не начинается с "Bearer ", добавляем префикс
                if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    context.Token = authHeader.Trim();
                }
                // Иначе используем стандартную обработку, которая извлечет токен после "Bearer "
            }
            
            return Task.CompletedTask;
        }
    };
});

// Регистрация сервисов
builder.Services.AddScoped<IUserSyncService, UserSyncService>();

// Регистрация grpc клиентов для сервисов
builder.Services.AddIamServiceClient(configuration);
builder.Services.AddFileServiceClient(configuration);
builder.Services.AddTrackServiceClient(configuration);
builder.Services.AddPlaylistServiceClient(configuration);
builder.Services.AddUserServiceClient(configuration);
builder.Services.AddAlbumServiceClient(configuration);

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.SetMinimumLevel(LogLevel.Debug);  // Изменено с Debug на Information для лучшей видимости
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

    // Добавляем поддержку авторизации в Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Token. Префикс 'Bearer' добавлять не обязательно. Пример: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.WebHost.ConfigureKestrel(o =>
{
    o.ListenAnyIP(8085, listenOptions => listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3);
});

var app = builder.Build();

// CORS должен быть до других middleware
app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// Middleware для обработки ошибок gRPC
app.UseMiddleware<GrpcExceptionMiddleware>();

// Добавляем middleware аутентификации и авторизации
app.UseAuthentication();
app.UseAuthorization();

// JWT middleware для проверки токенов (должен идти после Authentication, но до MapControllers)
app.UseMiddleware<JwtMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

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
                logger.LogError(exceptionHandlerFeature.Error, "UseExceptionHandler глобальная ошибка в WebHost");
            }
        }
        return Task.CompletedTask;
    });
});

app.Run();
