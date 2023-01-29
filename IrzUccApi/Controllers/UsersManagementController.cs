using IrzUccApi.Enums;
using IrzUccApi.Models.Db;
using IrzUccApi.Models.Requests.User;
using IrzUccApi.Models.Requests.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Controllers
{
    [Route("api/users_management")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsersManagementController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        public UsersManagementController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegUserRequest request)
        {
            if (await _userManager.FindByEmailAsync(request.Email) != null)
                return BadRequest(RequestErrorMessages.EmailAlreadyUsed);

            var user = new AppUser
            {
                FirstName = request.FirstName,
                Surname = request.Surname,
                Patronymic = request.Patronymic,
                UserName = request.Email,
                Email = request.Email,
                Birthday = request.Birthday
            };

            var identityResult = await _userManager.CreateAsync(user, request.Password);
            if (!identityResult.Succeeded)
                return BadRequest(identityResult.Errors);

            return Ok(user.Id);
        }

        [HttpPut("{id}/update_reg_info")]
        public async Task<IActionResult> UpdateUserRegInfo(string id, [FromBody] UpdateRegInfoRequest request)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            if (await _userManager.IsInRoleAsync(user, RolesNames.SuperAdmin))
                return Forbid();

            user.FirstName = request.FirstName;
            user.Surname = request.Surname;
            user.Patronymic = request.Patronymic;
            user.Email = request.Email;
            user.Birthday = request.Birthday;
            var identityResult = await _userManager.UpdateAsync(user);
            if (!identityResult.Succeeded)
                return BadRequest(identityResult.Errors);

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            if (await _userManager.IsInRoleAsync(user, RolesNames.SuperAdmin))
                return Forbid();

            var identityResult = await _userManager.DeleteAsync(user);
            if (!identityResult.Succeeded)
                return BadRequest(identityResult.Errors);

            return Ok();
        }

        [HttpPut("{id}/activate")]
        public async Task<IActionResult> Acivate(string id)
            => await ChangeActivation(id, true);

        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(string id)
            => await ChangeActivation(id, false);

        private async Task<IActionResult> ChangeActivation(string userId, bool activation)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            if (await _userManager.IsInRoleAsync(user, RolesNames.SuperAdmin))
                return Forbid();

            user.IsActiveAccount = activation;
            var identityResult = await _userManager.UpdateAsync(user);
            if (!identityResult.Succeeded)
                return BadRequest(identityResult.Errors);

            return Ok();
        }

        [HttpPut("{id}/change_password")]
        public async Task<IActionResult> ChangeUserPassword(string id, [FromBody][Required][MinLength(6)] string newPassword)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            if (await _userManager.IsInRoleAsync(user, RolesNames.SuperAdmin))
                return Forbid();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var identityResult = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!identityResult.Succeeded)
                return BadRequest(identityResult.Errors);

            return Ok();
        }
    }
}
