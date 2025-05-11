using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soundy.UserService.Entities;

namespace Soundy.UserService.DataAccess.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Email).IsRequired().HasMaxLength(128);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(128);

            builder.HasIndex(x => x.Email).IsUnique();
        }
    }
}
