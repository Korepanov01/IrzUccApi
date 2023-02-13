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

        private readonly JwtConfiguration _jwtConfiguration;
        private readonly UserManager<AppUser> _userManager;

        public JwtService(
            JwtConfiguration jwtConfiguration,
            UserManager<AppUser> userManager)
        {
            _jwtConfiguration = jwtConfiguration;
            _userManager = userManager;
        }

        public async Task<TokensDto> GenerateTokens(string email)
        {
            var tokenKey = Encoding.UTF8.GetBytes(_jwtConfiguration.SecurityKey);

            var user = await _userManager.FindByEmailAsync(email)
                       ?? throw new ArgumentException("There is no such user!", nameof(email));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity((await _userManager.GetRolesAsync(user))
                    .Select(r => new Claim(ClaimTypes.Role, r))
                    .Append(new Claim(ClaimTypes.NameIdentifier, user.Id))),
                Expires = DateTime.Now.AddMinutes(_jwtLifeTimeMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new TokensDto(
                tokenHandler.WriteToken(token),
                Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)));
        }

        public string GetIdFromExpiredJwt(string jwt)
        {
            var tokenKey = Encoding.UTF8.GetBytes(_jwtConfiguration.SecurityKey);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
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