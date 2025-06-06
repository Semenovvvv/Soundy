﻿using Microsoft.EntityFrameworkCore;
using Soundy.UserService.DataAccess;

namespace Soundy.UserService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureContext(this IServiceCollection services, IConfiguration configuration)
        {
            const string host = "USER_SERVICE_POSTGRES_HOST";
            const string db = "USER_SERVICE_POSTGRES_DB";
            const string port = "USER_SERVICE_POSTGRES_PORT";
            const string user = "USER_SERVICE_POSTGRES_USER";
            const string password = "USER_SERVICE_POSTGRES_PASSWORD";

            //var connectionString = configuration.GetConnectionString("DefaultConnection");

            var connectionString = $"Host={configuration[host] ?? throw new ArgumentNullException(host)};" +
                                   $"Port={configuration[port] ?? throw new ArgumentNullException(port)};" +
                                   $"Database={configuration[db] ?? throw new ArgumentNullException(db)};" +
                                   $"Username={configuration[user] ?? throw new ArgumentNullException(user)};" +
                                   $"Password={configuration[password] ?? throw new ArgumentNullException(password)}";

            services.AddDbContextFactory<DatabaseContext>(optionsBuilder
                => optionsBuilder.UseNpgsql(connectionString));

            return services;
        }
    }
}
