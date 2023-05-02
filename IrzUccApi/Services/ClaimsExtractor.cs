using System.Security.Claims;

namespace IrzUccApi.Services
{
    public static class ClaimsExtractor
    {
        public static string? GetNameIdentifier(ClaimsPrincipal claims)
            => claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
