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
        public async Task<IActionResult> GetUserPositions(string userId)
            => await GetUserPositionsByUserId(userId);

        [HttpGet("my")]
        public async Task<IActionResult> GetMyUserPositions()
        {
            var myId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (myId == null)
                return Unauthorized();
            return await GetUserPositionsByUserId(myId);
        }

        private async Task<IActionResult> GetUserPositionsByUserId(string id)
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
