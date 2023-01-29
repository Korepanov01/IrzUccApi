﻿using IrzUccApi.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IrzUccApi.Models.Dtos;
using System.Security.Claims;
using IrzUccApi.Models.Db;
using IrzUccApi.Models.Requests.User;
using IrzUccApi.Models.GetOptions;
using IrzUccApi.Models.Requests.Users;
using System.ComponentModel.DataAnnotations;

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
    public async Task<IActionResult> GetUsers([FromQuery] UserSearchParameters parameters)
    {
        var isAdminOrSuperAdmin = User.IsInRole(RolesNames.Admin) || User.IsInRole(RolesNames.SuperAdmin);

        var normalizedSearchString = parameters.SearchString?.ToUpper();
        parameters.IsActive = parameters.IsActive == null ? null : (!isAdminOrSuperAdmin ? true : parameters.IsActive);

        var users = await _dbContext.Users
            .Where(u => parameters.PositionId == null || parameters.PositionId == (u.Position != null ? u.Position.Id : 0))
            .Where(u => normalizedSearchString == null || u.FullNameEmail.Contains(normalizedSearchString))
            .Where(u => parameters.Role == null || u.UserRoles.Select(ur => ur.Role != null ? ur.Role.Name : "").Contains(parameters.Role))
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
                    u.Position != null ? u.Position.Name : null))
            .ToArrayAsync();

        return Ok(users);
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
    public async Task<IActionResult> UpdateUserRegInfo(string id, [FromBody] UpdateRegInfoRequest request)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();
        if (!User.IsInRole(RolesNames.SuperAdmin) && await _userManager.IsInRoleAsync(user, RolesNames.SuperAdmin))
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

    [HttpPut("{id}/change_password")]
    [Authorize(Policy = "AdminRights")]
    public async Task<IActionResult> ChangePassword(string id, [FromBody][Required][MinLength(6)] string newPassword)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        if (!User.IsInRole(RolesNames.SuperAdmin) && await _userManager.IsInRoleAsync(user, RolesNames.SuperAdmin))
            return Forbid();

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var identityResult = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (!identityResult.Succeeded)
            return BadRequest(identityResult.Errors);

        return Ok();
    }

    [HttpPut("update_extra_info")]
    [Authorize]
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

        if (await _userManager.IsInRoleAsync(user, RolesNames.SuperAdmin))
            return Forbid();

        user.IsActiveAccount = activation;
        var identityResult = await _userManager.UpdateAsync(user);
        if (!identityResult.Succeeded)
            return BadRequest(identityResult.Errors);

        return Ok();
    }

    [HttpPost("register")]
    [Authorize(Policy = "AdminRights")]
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
}