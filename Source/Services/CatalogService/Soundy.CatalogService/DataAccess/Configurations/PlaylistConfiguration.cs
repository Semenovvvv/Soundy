using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soundy.CatalogService.Entities;

namespace Soundy.CatalogService.DataAccess.Configurations
{
    public class PlaylistConfiguration : IEntityTypeConfiguration<Playlist>
    {
        public void Configure(EntityTypeBuilder<Playlist> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(x => x.AuthorId);

            builder.Navigation(x => x.Tracks)
                .AutoInclude();
        }
    }
}
