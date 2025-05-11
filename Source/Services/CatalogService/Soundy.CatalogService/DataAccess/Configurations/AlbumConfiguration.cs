using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soundy.CatalogService.Entities;

namespace Soundy.CatalogService.DataAccess.Configurations
{
    public class AlbumConfiguration : IEntityTypeConfiguration<Album>
    {
        public void Configure(EntityTypeBuilder<Album> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasMany(x => x.Tracks)
                .WithOne(x => x.Album);

            builder.Navigation(x => x.Tracks)
                .AutoInclude();
        }
    }
}
