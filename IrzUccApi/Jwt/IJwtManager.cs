using System.Security.Claims;

namespace IrzUccApi.Jwt
{
    public interface IJwtManager
    {
        Task<Tokens> GenerateTokens(string email);
        string GetEmailFromExpiredJwt(string jwt);
    }
}
