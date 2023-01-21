using System.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IrzUccApi.Models;
using Microsoft.AspNetCore.Identity;

namespace IrzUccApi.Jwt
{
    public class JwtManager : IJwtManager
    {
        private readonly IConfiguration _iConfiguration;
        private readonly UserManager<AppUser> _userManager;

        public JwtManager(IConfiguration iConfiguration, UserManager<AppUser> userManager)
        {
            _iConfiguration = iConfiguration;
            _userManager = userManager;
        }

        public async Task<Tokens> GenerateTokens(string email)
        {
            var tokenKey = Encoding.UTF8.GetBytes(_iConfiguration["JWT:SecurityKey"] 
                       ?? throw new ConfigurationErrorsException("JWT:SecurityKey is empty!"));
            
            var user = (await _userManager.FindByEmailAsync(email)) 
                       ?? throw new ArgumentException("There is no such user", email);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity((await _userManager.GetRolesAsync(user))
                    .Select(r => new Claim(ClaimTypes.Role, r))
                    .Append(new Claim(ClaimTypes.Email, email))),
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

        public string GetEmailFromExpiredJwt(string jwt)
        {
            var tokenKey = Encoding.UTF8.GetBytes(_iConfiguration["JWT:SecurityKey"]
                ?? throw new ConfigurationErrorsException("JWT:SecurityKey is empty!"));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
                ClockSkew = TimeSpan.Zero
            };

            var principal = new JwtSecurityTokenHandler().ValidateToken(jwt, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken 
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            
            return (principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email) ??
                    throw new SecurityTokenException("Invalid token")).Value;
        }
    }
}