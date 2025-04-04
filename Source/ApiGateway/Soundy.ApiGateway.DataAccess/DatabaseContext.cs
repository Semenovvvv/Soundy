using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Soundy.ApiGateway.DataAccess.Configurations;
using Soundy.ApiGateway.Domain.Entities;

namespace Soundy.ApiGateway.DataAccess
{
    internal class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
    {
        internal DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
        }
    }
}
