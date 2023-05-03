using IrzUccApi.Db.Models;
using IrzUccApi.Services;
using System.Security.Claims;

namespace IrzUccApi.Db.Repositories
{
    public class UserRepository : AppRepository<AppUser, AppDbContext>
    {
        public UserRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task<AppUser?> GetByClaimsAsync(ClaimsPrincipal claims)
        {
            var id = ClaimsExtractor.GetNameIdentifier(claims);
            if (id == null)
                return null;

            return await GetByIdAsync(Guid.Parse(id));
        }
    }
}
