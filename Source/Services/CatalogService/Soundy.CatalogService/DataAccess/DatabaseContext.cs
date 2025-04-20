using Microsoft.EntityFrameworkCore;
using Soundy.CatalogService.DataAccess.Configurations;
using Soundy.CatalogService.Entities;

namespace Soundy.CatalogService.DataAccess
{
    public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
    {
        internal DbSet<Track> Tracks { get; set; }
        internal DbSet<Playlist> Playlists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new TrackConfiguration());
            modelBuilder.ApplyConfiguration(new PlaylistConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
        }
    }
}
