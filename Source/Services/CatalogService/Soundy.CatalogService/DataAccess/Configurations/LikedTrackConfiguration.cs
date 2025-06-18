using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soundy.CatalogService.Entities;

namespace Soundy.CatalogService.DataAccess.Configurations
{
    public class LikedTrackConfiguration : IEntityTypeConfiguration<LikedTrack>
    {
        public void Configure(EntityTypeBuilder<LikedTrack> builder)
        {
            builder.HasKey(lt => new { lt.UserId, lt.TrackId });

            builder.HasOne(lt => lt.Track)
                .WithMany(t => t.LikedBy)
                .HasForeignKey(lt => lt.TrackId);

            builder.Property(lt => lt.LikedAt).IsRequired();
        }
    }
} 