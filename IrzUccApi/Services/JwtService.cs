using IrzUccApi.Models.Configurations;
using IrzUccApi.Models.Db;
using IrzUccApi.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IrzUccApi.Services
{
    public class JwtService
    {
        private readonly int _jwtLifeTimeMinutes = 20;
        private readonly SymmetricSecurityKey _symmetricSecurityKey;

        private readonly JwtConfiguration _jwtConfiguration;
        private readonly UserManager<AppUser> _userManager;

        public JwtService(
            JwtConfiguration jwtConfiguration,
            UserManager<AppUser> userManager)
        {
            _jwtConfiguration = jwtConfiguration;
            _userManager = userManager;
            _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.SecurityKey));
        }

        public async Task<TokensDto> GenerateTokensAsync(AppUser user)
        {
            var tokenDescriptor = await GetTokenDescriptorAsync(user);

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            var jwt = tokenHandler.WriteToken(securityToken);
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            return new TokensDto(jwt, refreshToken);
        }

        private async Task<SecurityTokenDescriptor> GetTokenDescriptorAsync(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = roles.Select(r => new Claim(ClaimTypes.Role, r))
                .Append(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));

            return new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtLifeTimeMinutes),
                SigningCredentials = new SigningCredentials(
                    _symmetricSecurityKey,
                    SecurityAlgorithms.HmacSha256Signature),
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow,
                Audience = _jwtConfiguration.Audience,
                Issuer = _jwtConfiguration.Issuer
            };
        }

        public string ExtractUserIdFromExpiredJwt(string jwt)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _symmetricSecurityKey,
                ClockSkew = TimeSpan.Zero
            };

            ClaimsPrincipal claimsPrincipal;
            SecurityToken securityToken;
            try
            {
                claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(jwt, tokenValidationParameters, out securityToken);
            }
            catch (Exception)
            {
                throw new SecurityTokenException();
            }
            if (securityToken is not JwtSecurityToken jwtSecurityToken
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException();

            return (claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier) ??
                    throw new SecurityTokenException()).Value;
        }
    }
}