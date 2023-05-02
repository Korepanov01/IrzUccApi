using IrzUccApi.Db.Models;

namespace IrzUccApi.Db.Repositories
{
    public class CabinetRepository : AppRepository<Cabinet, AppDbContext>
    {
        public CabinetRepository(AppDbContext dbContext) : base(dbContext) { }
    }
}
