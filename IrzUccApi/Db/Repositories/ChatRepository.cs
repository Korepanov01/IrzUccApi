using IrzUccApi.Db.Models;

namespace IrzUccApi.Db.Repositories
{
    public class ChatRepository : AppRepository<Chat, AppDbContext>
    {
        public ChatRepository(AppDbContext dbContext) : base(dbContext) { }
    }
}
