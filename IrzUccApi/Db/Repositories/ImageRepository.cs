using IrzUccApi.Db.Models;

namespace IrzUccApi.Db.Repositories
{
    public class ImageRepository : AppRepository<Image, AppDbContext>
    {
        public ImageRepository(AppDbContext dbContext) : base(dbContext) { }
    }
}
