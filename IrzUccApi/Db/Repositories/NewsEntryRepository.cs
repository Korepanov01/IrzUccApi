using IrzUccApi.Db.Models;

namespace IrzUccApi.Db.Repositories
{
    public class NewsEntryRepository : AppRepository<NewsEntry, AppDbContext>
    {
        public NewsEntryRepository(AppDbContext dbContext) : base(dbContext) { }
    }
}
