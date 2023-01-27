using IrzUccApi.Models;
using IrzUccApi.Enums;
using IrzUccApi.Models.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using IrzUccApi.Models.Dtos.Position;
using IrzUccApi.Models.Dtos;
using System.Security.Claims;

namespace IrzUccApi.Controllers;

[Route("api/users")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public UsersController(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
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
                    user.IsActiveAccount,
                    user.Image,
                    await _userManager.GetRolesAsync(user),
                    user.Position?.Name));
        }
        return Ok(userListItems);
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
            user.EmploymentDate,
            user.Position != null ? new PositionDto(user.Position.Id, user.Position.Name) : null,
            user.PositionHistoricalRecords
                .OrderBy(p => p.DateTime)
                .Select(p => new PositionHistoricalRecordDto(p.DateTime, p.PositionName))
                .ToArray()));
    }

    [HttpPut("{id}/update_reg_info")]
    [Authorize(Policy = "AdminRights")]
    public async Task<IActionResult> UpdateUserRegInfo(string id, [FromBody] UserRegInfo userRegInfo)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();
        if (!User.IsInRole(Roles.SuperAdmin) && await _userManager.IsInRoleAsync(user, Roles.SuperAdmin))
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

    [HttpPut("update_extra_info")]
    [Authorize]
    public async Task<IActionResult> UpdateUserExtraInfo([FromBody] UserExtraInfo userExtraInfo)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        user.Image = userExtraInfo.Image;
        user.AboutMyself = userExtraInfo.AboutMyself;
        user.MyDoings = userExtraInfo.MyDoings;
        user.Skills = userExtraInfo.Skills;
        var identityResult = await _userManager.UpdateAsync(user);
        if (!identityResult.Succeeded)
            return BadRequest(identityResult.Errors);

        return Ok();
    }

    [HttpPut("{id}/activate")]
    [Authorize(Policy = "AdminRights")]
    public async Task<IActionResult> Acivate(string id)
        => await ChangeActivation(id, true);

    [HttpPut("{id}/deactivate")]
    [Authorize(Policy = "AdminRights")]
    public async Task<IActionResult> Deactivate(string id)
        => await ChangeActivation(id, false);

    private async Task<IActionResult> ChangeActivation(string userId, bool activation)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        if (await _userManager.IsInRoleAsync(user, Roles.SuperAdmin))
            return Forbid();

        user.IsActiveAccount = activation;
        var identityResult = await _userManager.UpdateAsync(user);
        if (!identityResult.Succeeded)
            return BadRequest(identityResult.Errors);

        return Ok();
    }

    [HttpPost("register")]
    [Authorize(Policy = "AdminRights")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegInfo request)
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

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        if (await _userManager.IsInRoleAsync(user, Roles.SuperAdmin))
            return Forbid();

        var identityResult = await _userManager.DeleteAsync(user);
        if (!identityResult.Succeeded)
            return BadRequest(identityResult.Errors);

        return Ok();
    }
}