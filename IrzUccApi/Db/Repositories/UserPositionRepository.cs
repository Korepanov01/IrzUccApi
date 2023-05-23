using IrzUccApi.Db.Models;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.PagingOptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrzUccApi.Db.Repositories
{
    public class UserPositionRepository : AppRepository<UserPosition, AppDbContext>
    {
        public UserPositionRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<UserPositionDto>> GetDtosAsync(AppUser user)
        {
            return await _dbContext.UserPositions
                .Where(up => up.User.Id == user.Id)
                .OrderBy(up => up.Start)
                .Select(up => new UserPositionDto(
                    up.Id,
                    up.Start,
                    up.End,
                    new PositionDto(
                        up.Position.Id,
                        up.Position.Name)))
                .ToArrayAsync();
        }


        public async Task<UserPosition?> GetByPositionAndUserAsync(Position position, AppUser user)
            => await _dbContext.UserPositions.FirstOrDefaultAsync(up => up.End == null && up.Position.Id == position.Id && up.User.Id == user.Id);
    }
}
