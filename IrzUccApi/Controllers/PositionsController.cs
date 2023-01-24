using IrzUccApi.Enums;
using IrzUccApi.Models;
using IrzUccApi.Models.Dtos.Position;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Controllers
{
    [Route("api/positions")]
    [ApiController]
    [Authorize(Roles = "Admin,SuperAdmin")]
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
        public async Task<IActionResult> GetPositions(
            [Range(0, 50)] int pageSize = 10,
            [Range(1, int.MaxValue)] int page = 1,
            string? searchString = null)
            => Ok(await _dbContext.Positions
                .Where(p => searchString == null || p.Name.ToUpper().Contains(searchString.ToUpper()))
                .OrderBy(p => p.Name)
                .Select(p => new PositionDto(p.Id, p.Name))
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .ToArrayAsync());

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
            _dbContext.SaveChanges();

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
            _dbContext.SaveChanges();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var position = await _dbContext.Positions.FirstOrDefaultAsync(p => p.Id == id);
            if (position == null)
                return NotFound();
            
            if (position.Users.Count != 0)
                return BadRequest(RequestErrorMessages.ThereAreUsersWithThisPositionMessage);

            _dbContext.Remove(position);
            _dbContext.SaveChanges();

            return Ok();
        }

        [HttpPost("change_user_position")]
        public async Task<IActionResult> ChangeUserPosition([FromBody] ChangeUserPositionRequest request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
            if (user == null)
                return NotFound(RequestErrorMessages.UserDoesntExistsMessage);

            if (await _userManager.IsInRoleAsync(user, Roles.SuperAdmin))
                return Forbid();

            if (request.IsRemoving)
            {
                user.EmploymentDate = null;
                user.Position = null;
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }

            if (request.PositionId == null)
                return NotFound(RequestErrorMessages.PositionDoesntExistsMessage);

            var newPosition = await _dbContext.Positions.FirstOrDefaultAsync(p => p.Id == request.PositionId);
            if (newPosition == null)
                return NotFound(RequestErrorMessages.PositionDoesntExistsMessage);

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
    }
}
