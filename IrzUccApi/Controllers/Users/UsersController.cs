﻿using IrzUccApi.Db;
using IrzUccApi.Enums;
using IrzUccApi.Models.Db;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.GetOptions;
using IrzUccApi.Models.Requests.Images;
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
    public async Task<IActionResult> GetUsersAsync([FromQuery] UserSearchParameters parameters)
    {
        var users = _dbContext.Users.AsQueryable();

        users = parameters.IsActive != null && User.IsInRole(RolesNames.Admin)
            ? users.Where(u => u.IsActiveAccount == parameters.IsActive)
            : users.Where(u => u.IsActiveAccount);

        if (parameters.PositionId != null)
        {
            if (parameters.PositionId == Guid.Empty)
                users = users.Where(u => !u.UserPosition.Where(up => up.End == null).Any());
            else
                users = users.Where(u => u.UserPosition
                    .Where(up => up.End == null)
                    .Select(up => up.Position.Id)
                    .Contains((Guid)parameters.PositionId));
        }

        if (parameters.SearchString != null)
        {
            var normalizedSearchWords = parameters.SearchString
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(sw => sw.ToUpper());
            foreach (var word in normalizedSearchWords)
                users = users.Where(u => (u.FirstName + u.Surname + u.Patronymic + u.Email).ToUpper().Contains(word));
        }

        if (parameters.Role != null)
            users = users
                .Where(u => u.UserRoles
                    .Select(ur => ur.Role != null ? ur.Role.Name : "")
                    .Contains(parameters.Role));

        return Ok(await users
            .OrderBy(u => (u.FirstName + u.Surname + u.Patronymic + u.Email).ToUpper())
            .Skip(parameters.PageSize * (parameters.PageIndex - 1))
            .Take(parameters.PageSize)
            .Select(u => new UserListItemDto(
                    u.Id,
                    u.FirstName,
                    u.Surname,
                    u.Patronymic,
                    u.Email,
                    u.IsActiveAccount,
                    u.Image != null ? u.Image.Id : null,
                    u.UserRoles.Select(ur => ur.Role != null ? ur.Role.Name : ""),
                    u.UserPosition
                        .Where(up => up.End == null)
                        .Select(up => new PositionDto(
                            up.Position.Id,
                            up.Position.Name))))
            .ToArrayAsync());
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUserAsync()
    {
        var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (currentUserId == null)
            return Unauthorized();
        return await GetUserAsync(currentUserId);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserByIdAsync(Guid id)
       => await GetUserAsync(id.ToString());

    private async Task<IActionResult> GetUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return Unauthorized();

        return Ok(new UserDto(
            user.Id,
            user.FirstName,
            user.Surname,
            user.Patronymic,
            user.Birthday,
            user.Image?.Id,
            user.AboutMyself,
            user.MyDoings,
            user.Skills,
            user.UserPosition
                .Where(up => up.End == null)
                .Select(up => new PositionDto(
                    up.Position.Id,
                    up.Position.Name)),
            user.UserRoles.Select(ur => ur.Role != null ? ur.Role.Name : ""),
            user.Subscribers.Count,
            user.Subscriptions.Count,
            user.Email,
            user.IsActiveAccount,
            user.Subscribers.Contains(currentUser)));
    }

    [HttpPut("me/update_photo")]
    public async Task<IActionResult> UpdatePhotoAsync([FromBody] ImageRequest request)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return Unauthorized();

        var image = new Image
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Extension = request.Extension,
            Data = request.Data,
            Source = ImageSources.User,
            SourceId = currentUser.Id,
        };
        await _dbContext.Images.AddAsync(image);

        if (currentUser.Image != null)
            _dbContext.Remove(currentUser.Image);
        currentUser.Image = image;

        _dbContext.Update(currentUser);
        await _dbContext.SaveChangesAsync();

        return Ok(image.Id);
    }

    [HttpPut("me/delete_photo")]
    public async Task<IActionResult> DeletePhotoAsync()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return Unauthorized();

        if (currentUser.Image != null)
            _dbContext.Remove(currentUser.Image);

        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPut("me/update_info")]
    public async Task<IActionResult> UpdateExtraInfoAsync([FromBody] UpdateExtraInfoRequest request)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return Unauthorized();

        currentUser.AboutMyself = request.AboutMyself;
        currentUser.MyDoings = request.MyDoings;
        currentUser.Skills = request.Skills;

        var identityResult = await _userManager.UpdateAsync(currentUser);
        if (!identityResult.Succeeded)
            return BadRequest(identityResult.Errors);

        return Ok();
    }
}