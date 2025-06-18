using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Soundy.IAM;
using Soundy.IAM.Configurations;
using Soundy.IAM.Constants;
using Soundy.IAM.Controllers;
using Soundy.IAM.DataAccess;
using Soundy.IAM.Entities;
using Soundy.IAM.Extensions;
using Soundy.IAM.Interfaces;
using Soundy.IAM.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var configuration = builder.Configuration;

builder.Services.Configure<JwtConfig>(configuration.GetSection("JwtConfig"));

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

//builder.Services.AddDbContext<IamDbContext>(options =>
//    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAuthService, AuthService>();

// Конфигурация JWT аутентификации
var jwtConfig = configuration.GetSection("JwtConfig").Get<JwtConfig>();
var key = Encoding.ASCII.GetBytes(jwtConfig?.Secret ?? "defaultsecretkey");

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtConfig?.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtConfig?.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.ConfigureContext(configuration);
builder.Services.AddGrpc();

builder.WebHost.ConfigureKestrel(options =>
{
    var hostPort = configuration.GetValue("IAM_SERVICE_PORT", 5005);
    options.ListenAnyIP(hostPort, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<IamDbContext>();

    try
    {
        context.Database.Migrate();

        // Создание ролей по умолчанию
        if (!context.Roles.Any())
        {
            context.Roles.Add(new Soundy.IAM.Entities.Role { Id = Guid.NewGuid(), Name = "User", Description = "Пользователь"});
            context.Roles.Add(new Soundy.IAM.Entities.Role { Id = Guid.NewGuid(), Name = "Admin", Description = "Администратор"});
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration error: {ex.Message}");
    }
}

// Настройка middleware
app.UseAuthentication();
app.UseAuthorization();

// Включаем сжатие ответов
app.UseResponseCompression();

app.MapGrpcService<IAMGrpcController>();

app.Run();
