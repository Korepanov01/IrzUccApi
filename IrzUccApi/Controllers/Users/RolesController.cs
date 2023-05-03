using IrzUccApi.Db;
using IrzUccApi.Db.Models;
using IrzUccApi.Enums;
using IrzUccApi.ErrorDescribers;
using IrzUccApi.Models.Requests.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrzUccApi.Controllers.Users
{
    [Route("api/roles")]
    [ApiController]
    [Authorize(Roles = RolesNames.Admin)]
    public class RolesController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _dbContext;

        public RolesController(UserManager<AppUser> userManager, AppDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
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
        {
            var isSuperAdmin = User.IsInRole(RolesNames.SuperAdmin);

            if (request.Role == RolesNames.SuperAdmin || request.Role == RolesNames.Admin && !isSuperAdmin)
                return Forbid();

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return NotFound(new[] { RequestErrorDescriber.UserDoesntExist });

            if (await _userManager.IsInRoleAsync(user, RolesNames.SuperAdmin) && (!isSuperAdmin || request.Role == RolesNames.Admin))
                return Forbid();

            var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == request.Role);
            if (role == null)
                return NotFound(new[] { RequestErrorDescriber.ThereIsNoSuchRole});

            if (await _dbContext.UserRoles.AnyAsync(ur => ur.Role.Name == request.Role && ur.UserId == request.UserId))
            {
                return BadRequest(new[] { RequestErrorDescriber.UserAlreadyWithThisRole });
            }

            var userRole = new AppUserRole
            {
                User = user,
                Role = role,
            };

            await _dbContext.AddAsync(userRole);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("remove_from_user")]
        public async Task<IActionResult> RemoveUserRoleAsync([FromBody] AddRemoveRoleRequest request)
        {
            var isSuperAdmin = User.IsInRole(RolesNames.SuperAdmin);

            if (request.Role == RolesNames.SuperAdmin || request.Role == RolesNames.Admin && !isSuperAdmin)
                return Forbid();

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return NotFound(new[] { RequestErrorDescriber.UserDoesntExist });

            if (await _userManager.IsInRoleAsync(user, RolesNames.SuperAdmin) && (!isSuperAdmin || request.Role == RolesNames.Admin))
                return Forbid();

            var userRole = await _dbContext.UserRoles.FirstOrDefaultAsync(ur => ur.Role.Name == request.Role && ur.UserId == request.UserId);
            if (userRole == null)
            {
                return BadRequest(new[] { RequestErrorDescriber.UserIsNotWithThisRole });
            }

            _dbContext.Remove(userRole);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
