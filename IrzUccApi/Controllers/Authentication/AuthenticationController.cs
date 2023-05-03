using IrzUccApi.Db.Models;
using IrzUccApi.ErrorDescribers;
using IrzUccApi.Models.Configurations;
using IrzUccApi.Models.Requests.Authentication;
using IrzUccApi.Models.Requests.Users;
using IrzUccApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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
        public async Task<IActionResult> GetTokenAsync([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return NotFound(new[] { RequestErrorDescriber.UserDoesntExist });

            if (!user.IsActiveAccount)
                return BadRequest(new[] { RequestErrorDescriber.AccountDeactivated });

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
                return BadRequest(new[] { RequestErrorDescriber.WrongPassword });

            var tokens = await _jwtManager.GenerateTokensAsync(user);

            user.RefreshToken = tokens.RefreshToken;
            var identityResult = await _userManager.UpdateAsync(user);
            if (!identityResult.Succeeded)
                return BadRequest(identityResult.Errors.Select(e => new RequestError(e.Code, e.Description)));

            return Ok(tokens);
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
        {
            string userId;
            try
            {
                userId = _jwtManager.ExtractUserIdFromExpiredJwt(request.Jwt);
            }
            catch (SecurityTokenException)
            {
                return BadRequest(new[] { RequestErrorDescriber.WrongJwt });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return BadRequest(new[] { RequestErrorDescriber.WrongJwt });

            if (user.RefreshToken != request.RefreshToken)
                return BadRequest(new[] { RequestErrorDescriber.WrongRefreshToken });

            var newTokens = await _jwtManager.GenerateTokensAsync(user);

            user.RefreshToken = newTokens.RefreshToken;
            var identityResult = await _userManager.UpdateAsync(user);
            if (!identityResult.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError);

            return Ok(newTokens);
        }

        [HttpPut("change_password")]
        [Authorize]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest request)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var identityResult = await _userManager.ChangePasswordAsync(currentUser, request.CurrentPassword, request.NewPassword);
            if (!identityResult.Succeeded)
                return BadRequest(identityResult.Errors.Select(e => new RequestError(e.Code, e.Description)));

            return Ok();
        }

        [HttpPost("send_reset_password_url")]
        public async Task<IActionResult> SendResetPasswordUrlAsync([FromBody] SendResetPasswordTokenRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (token == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            var url = $"{Request.Scheme}://{Request.Host}/api/authentication/reset_password?Email={Uri.EscapeDataString(request.Email)}&Token={Uri.EscapeDataString(token)}";
            await _emailService.SendResetPasswordMessageAsync(request.Email, url);

            return Ok();
        }

        [HttpGet("reset_password")]
        public async Task<IActionResult> ResetPasswordAsync([FromQuery] ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return NotFound();

            var newPassword = PasswordGenerator.GenerateRandomPassword(_passwordConfiguration);

            var result = await _userManager.ResetPasswordAsync(user, request.Token, newPassword);
            if (!result.Succeeded)
                return BadRequest("Ссылка недействительна!");

            await _emailService.SendNewPasswordMessageAsync(request.Email, newPassword);

            return Ok("На вашу почту отправлен новый пароль.");
        }
    }
}
