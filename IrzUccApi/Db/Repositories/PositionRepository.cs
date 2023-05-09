using IrzUccApi.Db.Models;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.PagingOptions;
using Microsoft.EntityFrameworkCore;

namespace IrzUccApi.Db.Repositories
{
    public class PositionRepository : AppRepository<Position, AppDbContext>
    {
        public PositionRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<PositionDto>> GetDtoListAsync(SearchStringParameters parameters)
        {
            var positions = _dbContext.Positions.AsQueryable();

            if (parameters.SearchString != null)
            {
                var normalizedSearchWords = parameters.SearchString
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(sw => sw.ToUpper());
                foreach (var word in normalizedSearchWords)
                    positions = positions.Where(p => p.Name.ToUpper().Contains(word));
            }

            return await positions
                .OrderBy(p => p.Name)
                .Skip(parameters.PageSize * parameters.PageIndex)
                .Take(parameters.PageSize)
                .Select(p => new PositionDto(
                    p.Id,
                    p.Name))
                .ToArrayAsync();
        }

        public async Task<bool> ExistsAsync(string name)
            => await ExistsAsync(p => p.Name == name);

        public async Task<bool> OwnedByAnyUserAsync(Position position)
            => await _dbContext.UserPositions.Where(up => up.Position.Id == position.Id).AnyAsync();

        public async Task<bool> OwnedByUserAsync(Position position, AppUser user)
            => await _dbContext.UserPositions
            .Where(up => up.End == null && up.Position.Id == position.Id && up.User.Id == user.Id)
            .AnyAsync();

        public async Task AddPositionToUserAsync(Position position, AppUser user, DateTime start)
        {
            var userPosition = new UserPosition
            {
                Start = start.ToUniversalTime(),
                Position = position,
                User = user
            };
            await _dbContext.AddAsync(userPosition);
            await _dbContext.SaveChangesAsync();
        }
    }
}
