using IrzUccApi.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IrzUccApi
{
    public class UserIdentifier
    {
        private readonly UserManager<AppUser> _userManager;

        public UserIdentifier(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<AppUser?> GetCurrentUser(ClaimsPrincipal claimsPrincipal)
        {
            var isAuthenticated = claimsPrincipal.Identity?.IsAuthenticated ?? false;
            if (!isAuthenticated) return null;

            var id = claimsPrincipal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            if (id == null) return null;

            return await _userManager.FindByIdAsync(id);
        }
    }
}
