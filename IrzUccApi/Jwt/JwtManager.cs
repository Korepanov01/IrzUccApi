using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IrzUccApi.Jwt
{
    public class JwtManager : IJwtManager
    {
        private readonly IConfiguration _iConfiguration;

        public JwtManager(IConfiguration iConfiguration)
        {
            _iConfiguration = iConfiguration;
        }

        public Tokens GenerateTokens(string email)
        {
            var tokenKey = Encoding.UTF8.GetBytes(_iConfiguration["JWT:SecurityKey"] ?? throw new ArgumentNullException("JWT:SecurityKey is empty!")); ;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, email)
                }),
                Expires = DateTime.Now.AddMinutes(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new Tokens
            {
                Jwt = tokenHandler.WriteToken(token),
                RefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
            };
        }

        public ClaimsPrincipal GetPrincipalsFromJwt(string token)
        {
            var tokenKey = Encoding.UTF8.GetBytes(_iConfiguration["JWT:SecurityKey"] ?? throw new ArgumentNullException("JWT:SecurityKey is empty!"));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
                ClockSkew = TimeSpan.Zero
            };

            var principal = new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}