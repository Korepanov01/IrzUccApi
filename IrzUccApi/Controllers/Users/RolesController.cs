using IrzUccApi.Db;
using IrzUccApi.Db.Models;
using IrzUccApi.Enums;
using IrzUccApi.ErrorDescribers;
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
        private readonly UnitOfWork _unitOfWork;

        public RolesController(
            UserManager<AppUser> userManager, 
            UnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
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
        public async Task<IActionResult> AddUserRoleAsync([FromBody] AddRemoveRoleRequest request)
            => await AddRemoveUserRoleAsync(request, true);

        [HttpPost("remove_from_user")]
        public async Task<IActionResult> RemoveUserRoleAsync([FromBody] AddRemoveRoleRequest request)
            => await AddRemoveUserRoleAsync(request, false);

        private async Task<IActionResult> AddRemoveUserRoleAsync([FromBody] AddRemoveRoleRequest request, bool isAdding)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return NotFound(new[] { RequestErrorDescriber.UserDoesntExist });

            var isSuperAdmin = User.IsInRole(RolesNames.SuperAdmin);
            if (request.Role == RolesNames.SuperAdmin 
                || !isSuperAdmin && (request.Role == RolesNames.Admin || await _userManager.IsInRoleAsync(user, RolesNames.SuperAdmin)))
                return Forbid();

            if (isAdding)
            {
                var role = await _unitOfWork.Roles.GetByNameAsync(request.Role);
                if (role == null)
                    return NotFound(new[] { RequestErrorDescriber.ThereIsNoSuchRole });

                if (await _userManager.IsInRoleAsync(user, request.Role))
                    return BadRequest(new[] { RequestErrorDescriber.UserAlreadyWithThisRole });

                await _unitOfWork.Roles.AddRoleToUserAsync(role, user);
            }
            else
            {
                if (!await _userManager.IsInRoleAsync(user, request.Role))
                    return BadRequest(new[] { RequestErrorDescriber.UserIsNotWithThisRole });

                await _unitOfWork.Roles.RemoveRoleFromUserAsync(request.Role, request.UserId);
            }

            return Ok();
        }
    }
}
