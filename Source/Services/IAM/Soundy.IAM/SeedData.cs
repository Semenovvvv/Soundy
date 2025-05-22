using Microsoft.EntityFrameworkCore;
using Soundy.IAM.DataAccess;
using Soundy.IAM.Entities;

namespace Soundy.IAM
{
    public class SeedData(IamDbContext dbContext)
    {
        public IamDbContext _dbContext = dbContext;

        public async Task EnsureCreateRole(string role)
        {
            if (await dbContext.Roles.FirstOrDefaultAsync(x => x.Name == role) == null)
            {
                await dbContext.Roles.AddAsync(new Role()
                {
                    Name = role,
                    Description = role
                });
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
