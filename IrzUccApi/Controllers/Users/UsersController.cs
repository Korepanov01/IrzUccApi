using IrzUccApi.Enums;
using IrzUccApi.Models.Db;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.GetOptions;
using IrzUccApi.Models.Requests.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IrzUccApi.Controllers.Users;

[Route("api/users")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public UsersController(
        AppDbContext dbContext,
        UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] UserSearchParameters parameters)
    {
        var users = _dbContext.Users.AsQueryable();

        users = parameters.IsActive != null && User.IsInRole(RolesNames.Admin)
            ? users.Where(u => u.IsActiveAccount == parameters.IsActive)
            : users.Where(u => u.IsActiveAccount);

        if (parameters.PositionId != null)
        {
            if (parameters.PositionId == 0)
                users = users.Where(u => u.UserPosition.Where(up => up.IsActive).Count() == 0);
            else
                users = users.Where(u => u.UserPosition
                .Where(up => up.IsActive)
                .Select(up => up.Position.Id).Contains((int)parameters.PositionId));
        }

        if (parameters.SearchString != null)
        {
            var normalizedSearchString = parameters.SearchString.ToUpper();
            users = users.Where(u => (u.FirstName + u.Surname + u.Patronymic + u.Email).ToUpper().Contains(normalizedSearchString));
        }

        if (parameters.Role != null)
            users = users.Where(u => u.UserRoles.Select(ur => ur.Role != null ? ur.Role.Name : "").Contains(parameters.Role));

        return Ok(await users
            .OrderBy(u => u.FirstName + u.Surname + u.Patronymic + u.Email)
            .Skip(parameters.PageSize * (parameters.PageIndex - 1))
            .Take(parameters.PageSize)
            .Select(u => new UserListItemDto(
                    u.Id,
                    u.FirstName,
                    u.Surname,
                    u.Patronymic,
                    u.Email,
                    u.IsActiveAccount,
                    u.Image,
                    u.UserRoles.Select(ur => ur.Role != null ? ur.Role.Name : ""),
                    u.UserPosition.Where(up => up.IsActive).Select(up => new PositionDto(up.Position.Id, up.Position.Name))))
            .ToArrayAsync());
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyUser()
    {
        var myId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (myId == null)
            return Unauthorized();
        return await GetUser(myId);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
       => await GetUser(id);

    private async Task<IActionResult> GetUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        return Ok(new UserDto(
            user.Id,
            user.FirstName,
            user.Surname,
            user.Patronymic,
            user.Birthday,
            user.Image,
            user.AboutMyself,
            user.MyDoings,
            user.Skills,
            user.UserPosition
                .Where(up => up.IsActive)
                .Select(up => new PositionDto(up.Position.Id, up.Position.Name)),
            user.Subscribers.Count,
            user.Subscriptions.Count));
    }

    [HttpPut("me/update_info")]
    public async Task<IActionResult> UpdateUserExtraInfo([FromBody] UpdateExtraInfoRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        user.Image = request.Image;
        user.AboutMyself = request.AboutMyself;
        user.MyDoings = request.MyDoings;
        user.Skills = request.Skills;
        var identityResult = await _userManager.UpdateAsync(user);
        if (!identityResult.Succeeded)
            return BadRequest(identityResult.Errors);

        return Ok();
    }
}