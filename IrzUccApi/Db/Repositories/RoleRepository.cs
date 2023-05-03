using IrzUccApi.Db.Models;

namespace IrzUccApi.Db.Repositories
{
    public class RoleRepository : AppRepository<AppRole, AppDbContext>
    {
        public RoleRepository(AppDbContext dbContext) : base(dbContext) { }
    }
}
