using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soundy.CatalogService.Entities;

namespace Soundy.CatalogService.DataAccess.Configurations
{
    public class TrackConfiguration : IEntityTypeConfiguration<Track>
    {
        public void Configure(EntityTypeBuilder<Track> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.AlbumId);
            builder.HasIndex(x => x.AuthorId);

            builder.HasOne(x => x.Album)
                .WithMany(x => x.Tracks)
                .HasForeignKey(x => x.AlbumId);

            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.AlbumId).IsRequired();
            builder.Property(x => x.AuthorId).IsRequired();
            builder.Property(x => x.Duration).IsRequired();
            builder.Property(x => x.Title).HasMaxLength(50).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();

            builder.Navigation(x => x.Album)
                .AutoInclude();
        }
    }
}
