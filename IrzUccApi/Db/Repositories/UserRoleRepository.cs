using IrzUccApi.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace IrzUccApi.Db.Repositories
{
    public class UserRoleRepository : AppRepository<AppUserRole, AppDbContext>
    {
        public UserRoleRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task<AppUserRole?> GetByRoleAndUserAsync(AppRole role, AppUser user)
            => await _dbContext.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id);
    }
}
