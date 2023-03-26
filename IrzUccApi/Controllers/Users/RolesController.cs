using IrzUccApi.Enums;
using IrzUccApi.Models.Db;
using IrzUccApi.Models.Requests.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IrzUccApi.Controllers.Users
{
    [Route("api/roles")]
    [ApiController]
    [Authorize(Roles = RolesNames.Admin)]
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
                RolesNames.CabinetsManager,
            };

            if (User.IsInRole(RolesNames.SuperAdmin))
                roles.Add(RolesNames.Admin);

            return Ok(roles);
        }

        [HttpPost("add_to_user")]
        public async Task<IActionResult> AddUserRole([FromBody] AddRemoveRoleRequest request)
            => await AddRemoveUserRole(request);

        [HttpPost("remove_from_user")]
        public async Task<IActionResult> RemoveUserRole([FromBody] AddRemoveRoleRequest request)
            => await AddRemoveUserRole(request, true);

        private async Task<IActionResult> AddRemoveUserRole(AddRemoveRoleRequest request, bool isRemoving = false)
        {
            var isSuperAdmin = User.IsInRole(RolesNames.SuperAdmin);

            if (request.Role == RolesNames.SuperAdmin || request.Role == RolesNames.Admin && !isSuperAdmin)
                return Forbid();

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return NotFound(RequestErrorMessages.UserDoesntExistMessage);

            if (await _userManager.IsInRoleAsync(user, RolesNames.SuperAdmin) && (!isSuperAdmin || request.Role == RolesNames.Admin))
                return Forbid();

            try
            {
                var identityResult = isRemoving
                    ? await _userManager.RemoveFromRoleAsync(user, request.Role)
                    : await _userManager.AddToRoleAsync(user, request.Role);
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
