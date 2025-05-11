using Microsoft.EntityFrameworkCore;
using Soundy.IAM.Entities;

namespace Soundy.IAM.DataAccess
{
    public class DatabaseContext : DbContext
    {
        internal DbSet<User> Users { get; set; }
        internal DbSet<Role> Roles { get; set; }
        internal DbSet<UserRole> UserRoles { get; set; }
        internal DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
