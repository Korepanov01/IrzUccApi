using IrzUccApi.Models;
using IrzUccApi.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IrzUccApi.Controllers;

[Route("api/users")]
[ApiController]
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
    public Task<IActionResult> Get()
    {
        var isCurrentUserAdmin = User.IsInRole("Admin");
        var users = _dbContext.Users
            .Where(u => isCurrentUserAdmin || u.IsActiveAccount)
            .OrderBy(u => u.Surname).ToArray();

        return Task.FromResult<IActionResult>(Ok(users));
    }
        

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] RegisterRequest registerRequest)
    {
        if (await _userManager.FindByEmailAsync(registerRequest.Email) != null)
            return BadRequest("User already exists!");

        var user = new AppUser
        {
            FirstName = registerRequest.FirstName,
            Surname = registerRequest.Surname,
            Patronymic = registerRequest.Patronymic,
            UserName = registerRequest.Email,
            Email = registerRequest.Email,
            Birthday = registerRequest.Birthday,
            EmploymentDate = registerRequest.EmploymentDate,
            IsActiveAccount = true
        };

        var identityResult = await _userManager.CreateAsync(user, registerRequest.Password);
        if (!identityResult.Succeeded)
            return BadRequest(identityResult.Errors);

        return Ok();
    }
}