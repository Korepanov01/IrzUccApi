using IrzUccApi.Db.Models;

namespace IrzUccApi.Db.Repositories
{
    public class EventRepository : AppRepository<Event, AppDbContext>
    {
        public EventRepository(AppDbContext dbContext) : base(dbContext) { }
    }
}
