using System.Security.Claims;

namespace IrzUccApi.Services
{
    public static class ClaimsExtractor
    {
        public static string? ExtractId(ClaimsPrincipal claims)
            => claims.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}
