using IrzUccApi.Models;
using IrzUccApi.Models.Dbo;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        if (await _userManager.FindByEmailAsync(registerRequest.Email) != null)
            return BadRequest("User already exists!");

        var position = await _dbContext.Positions.FirstOrDefaultAsync(p => p.Id == registerRequest.PositionId);
        if (position == null)
            return BadRequest("There is no such position!");

        var user = new AppUser
        {
            FirstName = registerRequest.FirstName,
            Surname = registerRequest.Surname,
            Patronymic = registerRequest.Patronymic,
            UserName = registerRequest.Email,
            Email = registerRequest.Email,
            Birthday = registerRequest.Birthday,
            EmploymentDate = registerRequest.EmploymentDate,
            Position = position
        };

        var identityResult = await _userManager.CreateAsync(user, registerRequest.Password);
        if (!identityResult.Succeeded)
            return BadRequest(identityResult.Errors);

        return Ok();
    }
}