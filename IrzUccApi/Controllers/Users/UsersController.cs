using IrzUccApi.Db;
using IrzUccApi.Db.Models;
using IrzUccApi.Enums;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.GetOptions;
using IrzUccApi.Models.Requests.Images;
using IrzUccApi.Models.Requests.User;
using IrzUccApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IrzUccApi.Controllers.Users;

[Route("api/users")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public UsersController(
        UnitOfWork unitOfWork,
        UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsersAsync([FromQuery] UserSearchParameters parameters)
    {
        var users = await _unitOfWork.Users.GetDtoListAsync(parameters, User);

        return Ok(users);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUserAsync()
    {
        var currentUserId = ClaimsExtractor.ExtractId(User);
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

        return Ok(new UserDto(user, currentUser));
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
        await _unitOfWork.Images.AddAsync(image);

        if (currentUser.Image != null)
            await _unitOfWork.Images.RemoveAsync(currentUser.Image);
        currentUser.Image = image;

        await _unitOfWork.Users.UpdateAsync(currentUser);

        return Ok(image.Id);
    }

    [HttpPut("me/delete_photo")]
    public async Task<IActionResult> DeletePhotoAsync()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return Unauthorized();

        if (currentUser.Image != null)
            await _unitOfWork.Images.RemoveAsync(currentUser.Image);

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