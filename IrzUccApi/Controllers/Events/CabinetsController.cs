using IrzUccApi.Db;
using IrzUccApi.Enums;
using IrzUccApi.Models.Db;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.GetOptions;
using IrzUccApi.Models.PagingOptions;
using IrzUccApi.Models.Requests.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrzUccApi.Controllers.Events
{
    [Route("api/cabinets")]
    [ApiController]
    [Authorize(Roles = RolesNames.CabinetsManager)]
    public class CabinetsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public CabinetsController(AppDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetCabinets([FromQuery] CabinetsGetParameters parameters)
        {
            var cabinets = _dbContext.Cabinets.AsQueryable();

            if (parameters.SearchString != null)
            {
                var normalizedSearchString = parameters.SearchString.ToUpper();
                cabinets = cabinets.Where(n => n.Name.ToUpper().Contains(normalizedSearchString));
            }

            if (parameters.FreeOnly)
            {
                if (parameters.Start == null || parameters.End == null)
                    return BadRequest();
                if (parameters.Start > parameters.End)
                    return BadRequest();
                cabinets = cabinets.Where(c => c.Events.Where(e =>
                    parameters.Start < e.Start && parameters.End > e.Start
                        || parameters.Start > e.Start && parameters.End < e.End).Count() == 0);
            }

            return Ok(await cabinets
                .OrderBy(c => c.Name)
                .Skip(parameters.PageSize * (parameters.PageIndex - 1))
                .Take(parameters.PageSize)
                .Select(c => new CabinetDto(
                    c.Id,
                    c.Name))
                .ToArrayAsync());
        }

        [HttpGet("{id}/events")]
        public async Task<IActionResult> GetCabinetEvents(Guid id, [FromQuery] PagingParameters parameters)
        {
            var cabinet = await _dbContext.Cabinets.FirstOrDefaultAsync(c => c.Id == id);
            if (cabinet == null)
                return NotFound();

            return Ok(cabinet.Events
                .OrderBy(e => e.Start)
                .Skip(parameters.PageSize * (parameters.PageIndex - 1))
                .Take(parameters.PageSize)
                .Select(e => new EventListItemDto(
                    e.Id,
                    e.Title,
                    e.Start,
                    e.End,
                    null))
                .ToArray());
        }

        [HttpPost]
        public async Task<IActionResult> PostCabinet([FromBody] PostPutCabinetRequest request)
        {
            if (await _dbContext.Cabinets.FirstOrDefaultAsync(c => c.Name == request.Name) != null)
                return BadRequest(RequestErrorMessages.CabinetAlreadyExists);

            var cabinet = new Cabinet
            {
                Name = request.Name
            };

            await _dbContext.AddAsync(cabinet);
            await _dbContext.SaveChangesAsync();

            return Ok(cabinet.Id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCabinet(Guid id, [FromBody] PostPutCabinetRequest request)
        {
            var cabinet = await _dbContext.Cabinets.FirstOrDefaultAsync(c => c.Id == id);
            if (cabinet == null)
                return NotFound();

            if (await _dbContext.Cabinets.FirstOrDefaultAsync(c => c.Name == request.Name) != null)
                return BadRequest(RequestErrorMessages.CabinetAlreadyExists);

            cabinet.Name = request.Name;
            _dbContext.Update(cabinet);

            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCabinet(Guid id)
        {
            var cabinet = await _dbContext.Cabinets.FirstOrDefaultAsync(c => c.Id == id);
            if (cabinet == null)
                return NotFound();

            if (cabinet.Events.Count != 0)
                return BadRequest(RequestErrorMessages.CabinetIsBooked);

            _dbContext.Cabinets.Remove(cabinet);

            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
