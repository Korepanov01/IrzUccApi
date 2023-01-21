using IrzUccApi.Models;
using IrzUccApi.Models.Dtos.Position;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IrzUccApi.Controllers
{
    [Route("api/positions")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class PositionsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public PositionsController(AppDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetPositions()
        {
            return Ok(await _dbContext.Positions.Select(p => new PositionDto
            {
                Id = p.Id,
                Name= p.Name
            }).ToArrayAsync());
        }

        [HttpPost]
        public async Task<IActionResult> AddPosition([FromBody] PositionName positionName)
        {
            if (await _dbContext.Positions.FirstOrDefaultAsync(p => p.Name == positionName.Name) != null)
                return BadRequest("Position already exists!");

            var a = await _dbContext.Positions.AddAsync(new Position 
            { 
                Name = positionName.Name
            });
            _dbContext.SaveChanges();
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePosition(int id, [FromBody] PositionName positionName)
        {
            var position = await _dbContext.Positions.FirstOrDefaultAsync(p => p.Id == id);
            if (position == null)
                return NotFound();

            if (await _dbContext.Positions.FirstOrDefaultAsync(p => p.Name == positionName.Name) != null)
                return BadRequest("Position already exists!");

            position.Name = positionName.Name;
            _dbContext.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var position = await _dbContext.Positions.FirstOrDefaultAsync(p => p.Id == id);
            if (position == null)
                return NotFound();

            _dbContext.Remove(position);
            _dbContext.SaveChanges();
            return Ok();
        }
    }
}
