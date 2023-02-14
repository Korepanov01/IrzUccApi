using IrzUccApi.Db;
using IrzUccApi.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IrzUccApi.Controllers.Users
{
    [Route("api/user_positions")]
    [ApiController]
    [Authorize]
    public class UserPositionController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public UserPositionController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserPositions(Guid userId)
            => await GetUserPositionsByUserId(userId);

        [HttpGet("my")]
        public async Task<IActionResult> GetMyUserPositions()
        {
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null)
                return Unauthorized();
            return await GetUserPositionsByUserId(new Guid(currentUserId));
        }

        private async Task<IActionResult> GetUserPositionsByUserId(Guid id)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return NotFound();

            return Ok(user.UserPosition
                .OrderBy(up => up.Start)
                .Select(up => new UserPositionDto(up.Id, up.Start, up.End))
                .ToArray());
        }


    }
}
