using IrzUccApi.Models.Db;

namespace IrzUccApi.Db.Repositories
{
    public class PositionRepository : AppRepository<Position, AppDbContext>
    {
        public PositionRepository(AppDbContext dbContext) : base(dbContext) { }
    }
}
