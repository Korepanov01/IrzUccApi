using IrzUccApi.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace IrzUccApi.Db.Repositories
{
    public class RoleRepository : AppRepository<AppRole, AppDbContext>
    {
        public RoleRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task<AppRole?> GetByNameAsync(string name)
            => await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == name);

        public async Task AddRoleToUserAsync(AppRole role, AppUser user)
        {
            var userRole = new AppUserRole
            {
                User = user,
                Role = role,
            };

            await _dbContext.AddAsync(userRole);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> RemoveRoleFromUserAsync(string roleName, Guid userId)
        {
            var userRole = await _dbContext.UserRoles.FirstOrDefaultAsync(ur => ur.Role.Name == roleName && ur.UserId == userId);
            if (userRole == null)
                return false;

            _dbContext.Remove(userRole);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
