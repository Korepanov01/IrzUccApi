using IrzUccApi.Models.Db;

namespace IrzUccApi.Db.Repositories
{
    public class UserPositionRepository : AppRepository<UserPosition, AppDbContext>
    {
        public UserPositionRepository(AppDbContext dbContext) : base(dbContext) { }


    }
}
