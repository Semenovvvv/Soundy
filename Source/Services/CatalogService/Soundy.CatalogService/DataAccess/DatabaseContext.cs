using Microsoft.EntityFrameworkCore;
using Soundy.CatalogService.DataAccess.Configurations;
using Soundy.CatalogService.Entities;

namespace Soundy.CatalogService.DataAccess
{
    public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
    {
        internal DbSet<Track> Tracks { get; set; }
        internal DbSet<Playlist> Playlists { get; set; }
        internal DbSet<Album> Albums { get; set; }
        internal DbSet<PlaylistTrack> PlaylistsTracks { get; set; }
        internal DbSet<LikedTrack> LikedTracks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new TrackConfiguration());
            modelBuilder.ApplyConfiguration(new PlaylistConfiguration());
            modelBuilder.ApplyConfiguration(new AlbumConfiguration());
            modelBuilder.ApplyConfiguration(new PlaylistTrackConfiguration());
            modelBuilder.ApplyConfiguration(new LikedTrackConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
        }
    }
}
