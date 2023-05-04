using IrzUccApi.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace IrzUccApi.Db.Repositories
{
    public class UserPositionRepository : AppRepository<UserPosition, AppDbContext>
    {
        public UserPositionRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task<UserPosition?> GetByPositionAndUserAsunc(Position position, AppUser user)
            => await _dbContext.UserPositions.FirstOrDefaultAsync(up => up.End == null && up.Position.Id == position.Id && up.User.Id == user.Id);
    }
}
