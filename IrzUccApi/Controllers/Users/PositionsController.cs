using IrzUccApi.Db;
using IrzUccApi.Enums;
using IrzUccApi.Models.Db;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.PagingOptions;
using IrzUccApi.Models.Requests.Position;
using IrzUccApi.Models.Requests.Positions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Controllers.Users
{
    [Route("api/positions")]
    [ApiController]
    [Authorize(Roles = "Admin")]
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
        [Authorize]
        public async Task<IActionResult> GetPositions([FromQuery] SearchStringParameters parameters)
        {
            var positions = _dbContext.Positions.AsQueryable();

            if (parameters.SearchString != null)
            {
                var normalizedSearchString = parameters.SearchString.ToUpper();
                positions = positions.Where(p => p.Name.ToUpper().Contains(normalizedSearchString));
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
        public async Task<IActionResult> AddPosition([FromBody] AddUpdatePositionRequest request)
        {
            if (await _dbContext.Positions.FirstOrDefaultAsync(p => p.Name == request.Name) != null)
                return BadRequest(RequestErrorMessages.PositionAlreadyExistsMessage);

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
        public async Task<IActionResult> UpdatePosition(Guid id, [FromBody] AddUpdatePositionRequest request)
        {
            var position = await _dbContext.Positions.FirstOrDefaultAsync(p => p.Id == id);
            if (position == null)
                return NotFound();

            if (await _dbContext.Positions.FirstOrDefaultAsync(p => p.Name == request.Name) != null)
                return BadRequest(RequestErrorMessages.PositionAlreadyExistsMessage);

            position.Name = request.Name;
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("add_pos_to_user")]
        public async Task<IActionResult> AddPositionToUser([FromBody] AddPositionToUserRequest request)
        {
            var position = await _dbContext.Positions.FirstOrDefaultAsync(p => p.Id == request.PositionId);
            if (position == null)
                return NotFound(RequestErrorMessages.PositionDoesntExistMessage);

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
            if (user == null)
                return NotFound(RequestErrorMessages.UserDoesntExistMessage);

            if (user.UserPosition.Where(up => up.End == null && up.Position.Id == request.PositionId).Any())
                return BadRequest(RequestErrorMessages.UserAlreadyOnPosition);

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
        public async Task<IActionResult> RemoveUserPosition([FromBody] RemoveUserPositionRequest request)
        {
            var position = await _dbContext.Positions.FirstOrDefaultAsync(p => p.Id == request.PositionId);
            if (position == null)
                return NotFound(RequestErrorMessages.PositionDoesntExistMessage);

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
            if (user == null)
                return NotFound(RequestErrorMessages.UserDoesntExistMessage);

            var userPosition = user.UserPosition.FirstOrDefault(up => up.End == null && up.Position?.Id == request.PositionId);
            if (userPosition == null)
                return BadRequest(RequestErrorMessages.UserIsNotInPosition);

            if (request.End < userPosition.Start)
                return BadRequest(RequestErrorMessages.EndTimeIsLessThenStartTime);

            userPosition.End = request.End;
            _dbContext.Update(userPosition);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
