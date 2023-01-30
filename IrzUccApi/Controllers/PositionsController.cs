using IrzUccApi.Enums;
using IrzUccApi.Models;
using IrzUccApi.Models.Db;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.GetOptions;
using IrzUccApi.Models.PagingOptions;
using IrzUccApi.Models.Requests.Position;
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var position = await _dbContext.Positions.FirstOrDefaultAsync(p => p.Id == id);
            if (position == null)
                return NotFound();
            
            if (position.Users.Count != 0)
                return BadRequest(RequestErrorMessages.ThereAreUsersWithThisPositionMessage);

            _dbContext.Positions.Remove(position);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{id}/to_user")]
        public async Task<IActionResult> ChangeUserPosition(int id, [FromBody] string userId)
        {
            var position = await _dbContext.Positions.FirstOrDefaultAsync(p => p.Id == id);
            if (position == null)
                return NotFound(RequestErrorMessages.PositionDoesntExistMessage);

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return NotFound(RequestErrorMessages.UserDoesntExistMessage);

            if (await _userManager.IsInRoleAsync(user, RolesNames.SuperAdmin))
                return Forbid();

            var employmentDate = DateTime.UtcNow;

            user.EmploymentDate = employmentDate;
            user.Position = position;
            _dbContext.Update(user);

            var positionHistoricalRecord = new PositionHistoricalRecord
            {
                DateTime = employmentDate,
                PositionName = position.Name,
                User = user
            };
            await _dbContext.AddAsync(positionHistoricalRecord);

            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("remove_user_position")]
        public async Task<IActionResult> RemoveUserPosition([FromBody] string userId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return NotFound();

            if (await _userManager.IsInRoleAsync(user, RolesNames.SuperAdmin))
                return Forbid();

            user.EmploymentDate = null;
            user.Position = null;
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
