﻿using IrzUccApi.Models;
using IrzUccApi.Models.Db;
using IrzUccApi.Models.Requests.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IrzUccApi.Controllers
{
    [Route("api/jwt")]
    [ApiController]
    public class JwtController : ControllerBase
    {
        private readonly JwtManager _jwtManager;
        private readonly UserManager<AppUser> _userManager;

        public JwtController(JwtManager jwtManager, UserManager<AppUser> userManager)
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
                return BadRequest("Incorrect login or password!");

            var tokens = await _jwtManager.GenerateTokens(user.Email);

            user.RefreshToken = tokens.RefreshToken;
            var identityResult = await _userManager.UpdateAsync(user);
            if(!identityResult.Succeeded)
                return BadRequest(identityResult.Errors);

            return Ok(tokens);
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> GetTokenAsync([FromBody] Tokens tokens)
        {
            var email = _jwtManager.GetEmailFromExpiredJwt(tokens.Jwt);

            var user = await _userManager.FindByEmailAsync(email);
            if(user == null)
                return Unauthorized("Non existing user!");

            if (user.RefreshToken != tokens.RefreshToken)
                return Unauthorized("Invalid refresh token!");

            var newTokens = await _jwtManager.GenerateTokens(user.Email);

            user.RefreshToken = newTokens.RefreshToken;
            var identityResult = await _userManager.UpdateAsync(user);
            if (!identityResult.Succeeded)
                return BadRequest(identityResult.Errors);

            return Ok(newTokens);
        }
    }
}
