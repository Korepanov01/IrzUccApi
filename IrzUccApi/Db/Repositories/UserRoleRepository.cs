using IrzUccApi.Db.Models;

namespace IrzUccApi.Db.Repositories
{
    public class UserRoleRepository : AppRepository<AppUserRole, AppDbContext>
    {
        public UserRoleRepository(AppDbContext dbContext) : base(dbContext) { }
    }
}
