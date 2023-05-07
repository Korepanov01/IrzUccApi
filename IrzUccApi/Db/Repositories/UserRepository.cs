using IrzUccApi.Db.Models;
using IrzUccApi.Enums;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.GetOptions;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IrzUccApi.Db.Repositories
{
    public class UserRepository : AppRepository<AppUser, AppDbContext>
    {
        public UserRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<UserListItemDto>> GetDtoListAsync(UserSearchParameters parameters, ClaimsPrincipal claims)
        {
            var users = _dbContext.Users.AsQueryable();

            users = parameters.IsActive != null && claims.IsInRole(RolesNames.Admin)
                ? users.Where(u => u.IsActiveAccount == parameters.IsActive)
                : users.Where(u => u.IsActiveAccount);

            if (parameters.PositionId != null)
            {
                if (parameters.PositionId == Guid.Empty)
                    users = users.Where(u => !u.UserPosition.Where(up => up.End == null).Any());
                else
                    users = users.Where(u => u.UserPosition
                        .Where(up => up.End == null)
                        .Select(up => up.Position.Id)
                        .Contains((Guid)parameters.PositionId));
            }

            if (parameters.SearchString != null)
            {
                var normalizedSearchWords = parameters.SearchString
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(sw => sw.ToUpper());
                foreach (var word in normalizedSearchWords)
                    users = users.Where(u => (u.FirstName + u.Surname + u.Patronymic + u.Email).ToUpper().Contains(word));
            }

            if (parameters.Role != null)
                users = users
                    .Where(u => u.UserRoles
                        .Select(ur => ur.Role != null ? ur.Role.Name : "")
                        .Contains(parameters.Role));

            return await users
                .OrderBy(u => (u.FirstName + u.Surname + u.Patronymic + u.Email).ToUpper())
                .Skip(parameters.PageSize * (parameters.PageIndex - 1))
                .Take(parameters.PageSize)
                .Select(u => new UserListItemDto(
                        u.Id,
                        u.FirstName,
                        u.Surname,
                        u.Patronymic,
                        u.Email,
                        u.IsActiveAccount,
                        u.Image != null ? u.Image.Id : null,
                        u.UserRoles.Select(ur => ur.Role != null ? ur.Role.Name : ""),
                        u.UserPosition
                            .Where(up => up.End == null)
                            .Select(up => new PositionDto(
                                up.Position.Id,
                                up.Position.Name))))
                .ToArrayAsync();
        }
    }
}
