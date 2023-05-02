using IrzUccApi.Models.Db;

namespace IrzUccApi.Db.Repositories
{
    public class UserRepository : AppRepository<AppUser, AppDbContext>
    {
        public UserRepository(AppDbContext dbContext) : base(dbContext) { }
    }
}
