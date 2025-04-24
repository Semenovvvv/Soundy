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

            builder.HasIndex(x => x.PlaylistId);
            builder.HasIndex(x => x.UserId);

            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.PlaylistId).IsRequired();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.Duration).IsRequired();
            builder.Property(x => x.Title).HasMaxLength(50).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
        }
    }
}
