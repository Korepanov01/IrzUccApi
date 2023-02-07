using IrzUccApi.Models;
using IrzUccApi.Models.Configurations;
using IrzUccApi.Models.Db;
using IrzUccApi.Models.Requests.User;
using IrzUccApi.Models.Requests.Users;
using IrzUccApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IrzUccApi.Controllers.Authentication
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly JwtService _jwtManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly EmailService _emailService;
        private readonly PasswordConfiguration _passwordConfiguration;

        public AuthenticationController(
            JwtService jwtManager, 
            UserManager<AppUser> userManager,
            EmailService emailService,
            PasswordConfiguration passwordConfiguration)
        {
            _jwtManager = jwtManager;
            _userManager = userManager;
            _emailService = emailService;
            _passwordConfiguration = passwordConfiguration;
        }

        [HttpPost]
        [Route("authenticate")]
        public async Task<IActionResult> GetTokenAsync([FromBody] LoginRequest loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);

            if (!await _userManager.CheckPasswordAsync(user, loginRequest.Password))
                return BadRequest("Incorrect login or password!");

            var tokens = await _jwtManager.GenerateTokens(user.Email);

            user.RefreshToken = tokens.RefreshToken;
            var identityResult = await _userManager.UpdateAsync(user);
            if (!identityResult.Succeeded)
                return BadRequest(identityResult.Errors);

            return Ok(tokens);
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] Tokens tokens)
        {
            var email = _jwtManager.GetEmailFromExpiredJwt(tokens.Jwt);

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
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

        [HttpPut("change_password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var identityResult = await _userManager.ChangePasswordAsync(currentUser, request.CurrentPassword, request.NewPassword);
            if (!identityResult.Succeeded)
                return BadRequest(identityResult.Errors);

            return Ok();
        }

        [HttpPost("send_reset_password_url")]
        public async Task<IActionResult> SendResetPasswordUrl([FromBody] SendResetPasswordTokenRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (token == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            var url = $"{Request.Scheme}://{Request.Host}/api/authentication/reset_password?Email={Uri.EscapeDataString(request.Email)}&Token={Uri.EscapeDataString(token)}";
            await _emailService.SendResetPasswordMessage(request.Email, url);

            return Ok();
        }

        [HttpGet("reset_password")]
        public async Task<IActionResult> ResetPassword([FromQuery] ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return NotFound();

            var newPassword = PasswordGenerator.GenerateRandomPassword(_passwordConfiguration);

            var result = await _userManager.ResetPasswordAsync(user, request.Token, newPassword);
            if (!result.Succeeded)
                return BadRequest("Ссылка недействительна!");

            await _emailService.SendNewPasswordMessage(request.Email, newPassword);

            return Ok("На вашу почту отправлен новый пароль.");
        }
    }
}
