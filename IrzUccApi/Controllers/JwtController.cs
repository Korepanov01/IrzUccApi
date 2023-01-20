using IrzUccApi.Jwt;
using IrzUccApi.Models;
using IrzUccApi.Models.Dbo;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IrzUccApi.Controllers
{
    [Route("api/jwt")]
    [ApiController]
    public class JwtController : ControllerBase
    {
        private readonly IJwtManager _jwtManager;
        private readonly UserManager<AppUser> _userManager;

        public JwtController(IJwtManager jwtManager, UserManager<AppUser> userManager)
        {
            _jwtManager = jwtManager;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("authenticate")]
        public async Task<IActionResult> GetTokenAsync([FromBody]LoginRequest loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);

            if(!await _userManager.CheckPasswordAsync(user, loginRequest.Password))
                return Unauthorized("Неправильный логин или пароль!");

            var tokens = _jwtManager.GenerateTokens(user.Email);

            user.RefreshToken = tokens.RefreshToken;
            await _userManager.UpdateAsync(user);

            return Ok(tokens);
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> GetTokenAsync([FromBody] Tokens tokens)
        {
            var claims = _jwtManager.GetPrincipalsFromJwt(tokens.Jwt);
            var email = claims.FindFirst(ClaimTypes.Email)?.Value;
            if(email == null)
                return Unauthorized("Invalid attempt!");

            var user = await _userManager.FindByEmailAsync(email);
            if(user == null)
                return Unauthorized("Invalid attempt!");

            if (user.RefreshToken != tokens.RefreshToken)
                return Unauthorized("Invalid attempt!");

            var newTokens = _jwtManager.GenerateTokens(user.Email);

            user.RefreshToken = newTokens.RefreshToken;
            await _userManager.UpdateAsync(user);

            return Ok(newTokens);
        }
    }
}
