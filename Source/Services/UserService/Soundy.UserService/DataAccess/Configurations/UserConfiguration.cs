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
            //builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(128);
            builder.Property(x => x.Login).IsRequired().HasMaxLength(128);
            builder.Property(x => x.Phone).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Role).IsRequired();

            builder.HasIndex(x => x.Email).IsUnique();
        }
    }
}
