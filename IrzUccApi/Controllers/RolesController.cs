﻿using IrzUccApi.Enums;
using IrzUccApi.Models.Db;
using IrzUccApi.Models.Requests.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IrzUccApi.Controllers
{
    [Route("api/roles")]
    [ApiController]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class RolesController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        public RolesController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult GetRoles()
        {
            var roles = new List<string>()
            {
                RolesNames.Support,
                RolesNames.Publisher,
            };

            if (User.IsInRole(RolesNames.SuperAdmin))
                roles.Add(RolesNames.Admin);

            return Ok(roles);
        }

        [HttpPost("add_to_user")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> AddUserRole([FromBody] AddRemoveRoleRequest request)
            => await AddRemoveUserRole(request);

        [HttpPost("remove_from_user")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> RemoveUserRole([FromBody] AddRemoveRoleRequest request)
            => await AddRemoveUserRole(request, true);

        private async Task<IActionResult> AddRemoveUserRole(AddRemoveRoleRequest request, bool isRemoving = false)
        {
            if (request.Role == RolesNames.SuperAdmin || !User.IsInRole(RolesNames.SuperAdmin) && request.Role == RolesNames.Admin)
                return Forbid();

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return NotFound(RequestErrorMessages.UserDoesntExistsMessage);

            if (await _userManager.IsInRoleAsync(user, RolesNames.SuperAdmin))
                return Forbid();

            try
            {
                var identityResult = isRemoving ? await _userManager.RemoveFromRoleAsync(user, request.Role) : await _userManager.AddToRoleAsync(user, request.Role);
                if (!identityResult.Succeeded)
                    return BadRequest(identityResult.Errors);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }
    }
}
