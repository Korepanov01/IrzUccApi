using IrzUccApi.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace IrzUccApi.Db.Repositories
{
    public class RoleRepository : AppRepository<AppRole, AppDbContext>
    {
        public RoleRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task<AppRole?> GetByNameAsync(string name)
            => await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == name);

    }
}
