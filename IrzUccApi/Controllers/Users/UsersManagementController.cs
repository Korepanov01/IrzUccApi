﻿using IrzUccApi.Db.Models;
using IrzUccApi.Enums;
using IrzUccApi.ErrorDescribers;
using IrzUccApi.Models.Configurations;
using IrzUccApi.Models.Requests.User;
using IrzUccApi.Models.Requests.Users;
using IrzUccApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IrzUccApi.Controllers.Users
{
    [Route("api/users_management")]
    [ApiController]
    [Authorize(Roles = RolesNames.Admin)]
    public class UsersManagementController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly PasswordConfiguration _passwordConfiguration;
        private readonly EmailService _emailService;

        public UsersManagementController(
            UserManager<AppUser> userManager,
            EmailService emailService,
            PasswordConfiguration passwordConfiguration)
        {
            _userManager = userManager;
            _passwordConfiguration = passwordConfiguration;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegUserRequest request)
        {
            if (await _userManager.FindByEmailAsync(request.Email) != null)
                return BadRequest(new[] { RequestErrorDescriber.EmailAlreadyRegistered });

            var user = new AppUser
            {
                FirstName = request.FirstName,
                Surname = request.Surname,
                Patronymic = request.Patronymic,
                UserName = request.Email,
                Email = request.Email,
                Birthday = request.Birthday.ToUniversalTime(),
            };

            var password = PasswordGenerator.GenerateRandomPassword(_passwordConfiguration);

            var identityResult = await _userManager.CreateAsync(user, password);
            if (!identityResult.Succeeded)
                return BadRequest(identityResult.Errors.Select(e => new RequestError(e.Code, e.Description)));

            await _emailService.SendRegisterMessageAsync(request.Email, password);

            return Ok(user.Id);
        }

        [HttpPut("{id}/update_reg_info")]
        public async Task<IActionResult> UpdateUserRegInfoAsync(Guid id, [FromBody] UpdateRegInfoRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound();
            if (!User.IsInRole(RolesNames.SuperAdmin) && await _userManager.IsInRoleAsync(user, RolesNames.SuperAdmin))
                return Forbid();

            user.FirstName = request.FirstName;
            user.Surname = request.Surname;
            user.Patronymic = request.Patronymic;
            user.Birthday = request.Birthday.ToUniversalTime();

            var identityResult = await _userManager.UpdateAsync(user);
            if (!identityResult.Succeeded)
                return BadRequest(identityResult.Errors);

            return Ok();
        }

        [HttpPut("{id}/activate")]
        public async Task<IActionResult> AcivateAsync(Guid id)
            => await ChangeActivationAsync(id, true);

        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> DeactivateAsync(Guid id)
            => await ChangeActivationAsync(id, false);

        private async Task<IActionResult> ChangeActivationAsync(Guid userId, bool activation)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return NotFound();

            if (await _userManager.IsInRoleAsync(user, RolesNames.SuperAdmin))
                return Forbid();

            user.IsActiveAccount = activation;
            var identityResult = await _userManager.UpdateAsync(user);
            if (!identityResult.Succeeded)
                return BadRequest(identityResult.Errors.Select(e => new RequestError(e.Code, e.Description)));

            return Ok();
        }
    }
}
