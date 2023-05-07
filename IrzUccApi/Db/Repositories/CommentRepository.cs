using IrzUccApi.Db.Models;

namespace IrzUccApi.Db.Repositories
{
    public class CommentRepository : AppRepository<Comment, AppDbContext>
    {
        public CommentRepository(AppDbContext dbContext) : base(dbContext) { }
    }
}
