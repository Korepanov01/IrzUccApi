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

        public async Task<UserPosition?> GetByPositionAndUserAsunc(Position position, AppUser user)
            => await _dbContext.UserPositions.FirstOrDefaultAsync(up => up.End == null && up.Position.Id == position.Id && up.User.Id == user.Id);

        public IEnumerable<UserListItemDto> GetSubscriptionsOrSubscribers(AppUser user, PagingParameters parameters, bool isSubscriptions)
        {
            var resultUsers = isSubscriptions ? user.Subscriptions : user.Subscribers;
            return resultUsers
                .OrderBy(u => u.FirstName + u.Surname + u.Patronymic + u.Email)
                .Skip(parameters.PageSize * parameters.PageIndex)
                .Take(parameters.PageSize)
                .Select(u => new UserListItemDto(
                        u.Id,
                        u.FirstName,
                        u.Surname,
                        u.Patronymic,
                        u.Email,
                        u.IsActiveAccount,
                        u.Image?.Id,
                        u.UserRoles.Select(ur => ur.Role != null ? ur.Role.Name : ""),
                        u.UserPosition.Where(up => up.End == null).Select(up => new PositionDto(up.Position.Id, up.Position.Name))));
        }
    }
}
