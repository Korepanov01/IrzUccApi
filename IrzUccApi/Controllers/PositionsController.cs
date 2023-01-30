using IrzUccApi.Enums;
using IrzUccApi.Models;
using IrzUccApi.Models.Db;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.GetOptions;
using IrzUccApi.Models.PagingOptions;
using IrzUccApi.Models.Requests.Position;
using IrzUccApi.Models.Requests.Positions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Controllers
{
    [Route("api/positions")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class PositionsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public PositionsController(AppDbContext dbContext, UserManager<AppUser> userManager)
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
                .Select(p => new PositionDto(p.Id, p.Name))
                .ToArrayAsync());
        }

        [HttpPost]
        public async Task<IActionResult> AddPosition(
            [FromBody][Required(AllowEmptyStrings = false)][MaxLength(100)] string name)
        {
            if (await _dbContext.Positions.FirstOrDefaultAsync(p => p.Name == name) != null)
                return BadRequest(RequestErrorMessages.PositionAlreadyExistsMessage);

            await _dbContext.Positions.AddAsync(new Position
            {
                Name = name
            });
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePosition(
            int id,
            [FromBody][Required(AllowEmptyStrings = false)][MaxLength(100)] string newName)
        {
            var position = await _dbContext.Positions.FirstOrDefaultAsync(p => p.Id == id);
            if (position == null)
                return NotFound();

            if (await _dbContext.Positions.FirstOrDefaultAsync(p => p.Name == newName) != null)
                return BadRequest(RequestErrorMessages.PositionAlreadyExistsMessage);

            position.Name = newName;
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

            if (user.UserPosition.FirstOrDefault(up => up.IsActive && up.Position?.Id == request.PositionId) != null)
                return BadRequest(RequestErrorMessages.UserAlreadyOnPosition);

            if (await _userManager.IsInRoleAsync(user, RolesNames.SuperAdmin))
                return Forbid();

            var userPosition = new UserPosition
            {
                Start = request.Start,
                Position = position,
                User = user,
                IsActive = true
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

            var posHistRec = user.UserPosition.FirstOrDefault(up => up.IsActive && up.Position?.Id == request.PositionId);
            if (posHistRec == null)
                return BadRequest(RequestErrorMessages.UserIsNotInPosition);

            posHistRec.IsActive = false;
            _dbContext.Update(posHistRec);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
