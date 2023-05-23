using IrzUccApi.Db;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IrzUccApi.Controllers.Users
{
    [Route("api/user_positions")]
    [ApiController]
    [Authorize]
    public class UserPositionController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public UserPositionController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserPositionsAsync(Guid userId)
            => await GetUserPositionsByUserIdAsync(userId);

        [HttpGet("my")]
        public async Task<IActionResult> GetMyUserPositionsAsync()
        {
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null)
                return Unauthorized();
            return await GetUserPositionsByUserIdAsync(new Guid(currentUserId));
        }

        private async Task<IActionResult> GetUserPositionsByUserIdAsync(Guid id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            var userPositions = await _unitOfWork.UserPositions.GetDtosAsync(user);

            return Ok(userPositions);
        }
    }
}
