using IrzUccApi.Db;
using IrzUccApi.Db.Models;
using IrzUccApi.Enums;
using IrzUccApi.ErrorDescribers;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.PagingOptions;
using IrzUccApi.Models.Requests.Position;
using IrzUccApi.Models.Requests.Positions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrzUccApi.Controllers.Users
{
    [Route("api/positions")]
    [ApiController]
    [Authorize]
    public class PositionsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public PositionsController(
            AppDbContext dbContext,
            UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetPositionsAsync([FromQuery] SearchStringParameters parameters)
        {
            var positions = _dbContext.Positions.AsQueryable();

            if (parameters.SearchString != null)
            {
                var normalizedSearchWords = parameters.SearchString
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(sw => sw.ToUpper());
                foreach (var word in normalizedSearchWords)
                    positions = positions.Where(p => p.Name.ToUpper().Contains(word));
            }

            return Ok(await positions
                .OrderBy(p => p.Name)
                .Skip(parameters.PageSize * (parameters.PageIndex - 1))
                .Take(parameters.PageSize)
                .Select(p => new PositionDto(
                    p.Id,
                    p.Name))
                .ToArrayAsync());
        }

        [HttpPost]
        [Authorize(Roles = RolesNames.Admin)]
        public async Task<IActionResult> AddPositionAsync([FromBody] AddUpdatePositionRequest request)
        {
            if (await _dbContext.Positions.FirstOrDefaultAsync(p => p.Name == request.Name) != null)
                return BadRequest(new[] { RequestErrorDescriber.PositionAlreadyExists });

            var position = new Position
            {
                Name = request.Name
            };

            await _dbContext.AddAsync(position);
            await _dbContext.SaveChangesAsync();

            return Ok(new PositionDto(
                position.Id,
                position.Name));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = RolesNames.Admin)]
        public async Task<IActionResult> UpdatePositionAsync(Guid id, [FromBody] AddUpdatePositionRequest request)
        {
            var position = await _dbContext.Positions.FirstOrDefaultAsync(p => p.Id == id);
            if (position == null)
                return NotFound();

            if (await _dbContext.Positions.FirstOrDefaultAsync(p => p.Name == request.Name) != null)
                return BadRequest(new[] { RequestErrorDescriber.PositionAlreadyExists });

            position.Name = request.Name;
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = RolesNames.Admin)]
        public async Task<IActionResult> DeletePositionAsync(Guid id)
        {
            var position = await _dbContext.Positions.FirstOrDefaultAsync(p => p.Id == id);
            if (position == null)
                return NotFound();

            if (await _dbContext.UserPositions.Where(up => up.Position == position).AnyAsync())
                return BadRequest(new[] { RequestErrorDescriber.ThereAreUsersWithThisPosition });

            _dbContext.Remove(position);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("add_pos_to_user")]
        [Authorize(Roles = RolesNames.Admin)]
        public async Task<IActionResult> AddPositionToUserAsync([FromBody] AddPositionToUserRequest request)
        {
            var position = await _dbContext.Positions.FirstOrDefaultAsync(p => p.Id == request.PositionId);
            if (position == null)
                return NotFound(new[] { RequestErrorDescriber.PositionDoesntExist });

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
            if (user == null)
                return NotFound(new[] { RequestErrorDescriber.UserDoesntExist });

            if (user.UserPosition.Where(up => up.End == null && up.Position.Id == request.PositionId).Any())
                return BadRequest(new[] { RequestErrorDescriber.UserAlreadyOnPosition });

            var userPosition = new UserPosition
            {
                Start = request.Start,
                Position = position,
                User = user
            };
            await _dbContext.AddAsync(userPosition);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("remove_user_position")]
        [Authorize(Roles = RolesNames.Admin)]
        public async Task<IActionResult> RemoveUserPositionAsync([FromBody] RemoveUserPositionRequest request)
        {
            var position = await _dbContext.Positions.FirstOrDefaultAsync(p => p.Id == request.PositionId);
            if (position == null)
                return NotFound(new[] { RequestErrorDescriber.PositionDoesntExist });

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
            if (user == null)
                return NotFound(new[] { RequestErrorDescriber.UserDoesntExist });

            var userPosition = user.UserPosition.FirstOrDefault(up => up.End == null && up.Position?.Id == request.PositionId);
            if (userPosition == null)
                return BadRequest(new[] { RequestErrorDescriber.UserIsNotInPosition });

            if (request.End < userPosition.Start)
                return BadRequest(new[] { RequestErrorDescriber.EndTimeIsLessThenStartTime });

            userPosition.End = request.End;
            _dbContext.Update(userPosition);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
