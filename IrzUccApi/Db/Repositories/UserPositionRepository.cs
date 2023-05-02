using IrzUccApi.Db.Models;

namespace IrzUccApi.Db.Repositories
{
    public class UserPositionRepository : AppRepository<UserPosition, AppDbContext>
    {
        public UserPositionRepository(AppDbContext dbContext) : base(dbContext) { }


    }
}
