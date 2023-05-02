using IrzUccApi.Db.Models;

namespace IrzUccApi.Db.Repositories
{
    public class MessageRepository : AppRepository<Message, AppDbContext>
    {
        public MessageRepository(AppDbContext dbContext) : base(dbContext) { }
    }
}
