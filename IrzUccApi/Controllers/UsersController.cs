using IrzUccApi.Models;
using IrzUccApi.Models.Dto;
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
    private const string UserDoesntExistsMessage = "User does not exist!";
    private const string UserAlreadyExistsMessage = "User already exists!";
    private const string PositionDoesntExistsMessage = "Position does not exists!";

    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public UsersController(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var isAdminOrSuperAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
        var users = await _dbContext.Users
            .Where(u => isAdminOrSuperAdmin || u.IsActiveAccount)
            .OrderBy(u => u.Surname).ToArrayAsync();
        var usersDto = new List<UserDto>();
        foreach (var u in users) 
        {
            usersDto.Add(new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                Surname = u.Surname,
                IsActiveAccount = isAdminOrSuperAdmin ? u.IsActiveAccount : null,
                Patronymic = u.Patronymic,
                EmploymentDate = u.EmploymentDate,
                Birthday = u.Birthday,
                Image = u.Image,
                Position = u.Position != null ? new Models.Dtos.Position.PositionDto
                {
                    Id = u.Position.Id,
                    Name = u.Position.Name
                } : null,
                Roles = await _userManager.GetRolesAsync(u)
            });
        }

        return Ok(usersDto);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) != null)
            return BadRequest(UserAlreadyExistsMessage);

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

    [HttpPut("{id}/change_position")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> ChangeUserPosition(string id, [FromBody] int positionId)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound(UserDoesntExistsMessage);

        var newPosition = await _dbContext.Positions.FirstAsync(p => p.Id == positionId);
        if (newPosition == null)
            return NotFound(PositionDoesntExistsMessage);

        var employmentDate = DateTime.UtcNow;
        user.EmploymentDate = employmentDate;
        user.Position = newPosition;
        _dbContext.Update(user);

        var positionHistoricalRecord = new PositionHistoricalRecord
        {
            DateTime = employmentDate,
            PositionName = newPosition.Name,
            User = user
        };
        await _dbContext.AddAsync(positionHistoricalRecord);

        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPut("{id}/delete_position")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> DeleteUserPosition(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound(UserDoesntExistsMessage);

        user.EmploymentDate = null;
        user.Position = null;
        _dbContext.Update(user);

        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPut("{id}/add_role")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> AddUserRole(string id, [FromBody][Required] string newRole)
    {
        if (newRole == "SuperAdmin" || !User.IsInRole("SuperAdmin") && newRole == "Admin")
            return StatusCode(StatusCodes.Status403Forbidden);

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound(UserDoesntExistsMessage);

        if (await _userManager.IsInRoleAsync(user, "SuperAdmin"))
            return StatusCode(StatusCodes.Status403Forbidden);

        try
        {
            var identityResult = await _userManager.AddToRoleAsync(user, newRole);
            if (!identityResult.Succeeded)
                return BadRequest(identityResult.Errors);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }

        return Ok();
    }

    [HttpPut("{id}/remove_role")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> RemoveUserRole(string id, [FromBody][Required] string role)
    {
        if (!User.IsInRole("SuperAdmin") && role == "Admin")
            return StatusCode(StatusCodes.Status403Forbidden);

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound(UserDoesntExistsMessage);

        if (await _userManager.IsInRoleAsync(user, "SuperAdmin"))
            return StatusCode(StatusCodes.Status403Forbidden);

        var identityResult = await _userManager.RemoveFromRoleAsync(user, role);
        if (!identityResult.Succeeded)
            return BadRequest(identityResult.Errors);

        return Ok();
    }
}