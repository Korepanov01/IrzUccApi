using IrzUccApi.Models;
using IrzUccApi.Enums;
using IrzUccApi.Models.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Controllers;

[Route("api/users")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;
    private readonly UserIdentifier _userIdentifier;

    public UsersController(AppDbContext dbContext, UserManager<AppUser> userManager, UserIdentifier userIdentifier)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _userIdentifier = userIdentifier;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers(
            [Range(0, 50)] int pageSize = 10,
            [Range(1, int.MaxValue)] int page = 1,
            string? searchString = null)
    {
        var isAdminOrSuperAdmin = User.IsInRole(Roles.Admin) || User.IsInRole(Roles.SuperAdmin);
        var users = await _dbContext.Users
                .Where(u => (isAdminOrSuperAdmin || u.IsActiveAccount)
                    && (searchString == null || (u.FirstName + u.Surname + u.Patronymic ?? "" + u.Email).ToUpper().Contains(searchString.ToUpper())))
                .OrderBy(u => (u.Surname + u.FirstName + u.Patronymic ?? ""))
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .ToArrayAsync();
        var userListItems= new List<UserListItemDto>();
        foreach (var user in users) 
        {
            userListItems.Add(new UserListItemDto(
                    user.Id,
                    user.FirstName,
                    user.Surname,
                    user.Patronymic,
                    user.Email,
                    isAdminOrSuperAdmin ? user.IsActiveAccount : null,
                    user.Image,
                    isAdminOrSuperAdmin ? await _userManager.GetRolesAsync(user) : null,
                    user.Position?.Name));
        }
        return Ok(userListItems);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserInfo(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound(RequestErrorMessages.UserDoesntExistsMessage);

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
            user.EmploymentDate,
            user.Position?.Name,
            user.PositionHistoricalRecords
                .OrderBy(p => p.DateTime)
                .Select(p => p.DateTime.ToString("yy.MM.dd") + " " + p.PositionName)
                .ToArray()));
    }

    [HttpPut("{id}/change_reg_info")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> UpdateUserRegInfo(string id, [FromBody] UserRegInfo userRegInfo)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound(RequestErrorMessages.UserDoesntExistsMessage);
        if (await _userManager.IsInRoleAsync(user, Roles.SuperAdmin))
            return Forbid();

        user.FirstName = userRegInfo.FirstName;
        user.Surname = userRegInfo.Surname;
        user.Patronymic = userRegInfo.Patronymic;
        user.Email = userRegInfo.Email;
        user.Birthday = userRegInfo.Birthday;
        var identityResult = await _userManager.UpdateAsync(user);
        if (!identityResult.Succeeded)
            return BadRequest(identityResult.Errors);

        return Ok();
    }

    [HttpPut("change_extra_info")]
    [Authorize]
    public async Task<IActionResult> UpdateUserExtraInfo([FromBody] UserExtraInfo userExtraInfo)
    {
        var user = await _userIdentifier.GetCurrentUser(User);
        if (user == null)
            return Unauthorized();

        if (await _userManager.IsInRoleAsync(user, Roles.SuperAdmin))
            return Forbid();

        user.Image = userExtraInfo.Image;
        user.AboutMyself = userExtraInfo.AboutMyself;
        user.MyDoings = userExtraInfo.MyDoings;
        user.Skills = userExtraInfo.Skills;
        var identityResult = await _userManager.UpdateAsync(user);
        if (!identityResult.Succeeded)
            return BadRequest(identityResult.Errors);

        return Ok();
    }

    [HttpPut("{id}/change_activation")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> ChangeUserActivation(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound(RequestErrorMessages.UserDoesntExistsMessage);

        if (await _userManager.IsInRoleAsync(user, Roles.SuperAdmin)
            || User.IsInRole(Roles.Admin) && await _userManager.IsInRoleAsync(user, Roles.Admin))
            return Forbid();

        user.IsActiveAccount = !user.IsActiveAccount;
        var identityResult = await _userManager.UpdateAsync(user);
        if (!identityResult.Succeeded)
            return BadRequest(identityResult.Errors);

        return Ok();
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegInfo request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) != null)
            return BadRequest(RequestErrorMessages.UserAlreadyExistsMessage);

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

        return Ok();
    }
}