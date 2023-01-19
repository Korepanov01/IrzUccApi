using System.Security.Claims;

namespace IrzUccApi.Jwt
{
    public interface IJwtManager
    {
        Tokens GenerateTokens(string email);
        ClaimsPrincipal GetPrincipalsFromJwt(string jwt);
    }
}
