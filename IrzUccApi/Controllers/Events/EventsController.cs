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
    [Route("api/events")]
    [ApiController]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public EventsController(AppDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyEventsAsync([FromQuery] PagingParameters parameters)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            return Ok(currentUser.Events
                .OrderBy(e => e.Start)
                .Skip(parameters.PageSize * (parameters.PageIndex - 1))
                .Take(parameters.PageSize)
                .Select(e => new EventListItemDto(
                    e.Id,
                    e.Title,
                    e.Start,
                    e.End,
                    e.Cabinet?.Name)));
        }

        [HttpGet("listenning")]
        public async Task<IActionResult> GetListenningEventsAsync([FromQuery] TimeRangeGetParameters parameters)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            if (parameters.Start > parameters.End || (parameters.End - parameters.Start).Days > 40)
                return BadRequest();

            return Ok(currentUser.ListeningEvents
                .OrderBy(e => e.Start)
                .Where(e => parameters.Start < e.Start && parameters.End > e.Start
                    || parameters.Start > e.Start && parameters.End < e.End)
                .Select(e => new EventListItemDto(
                    e.Id,
                    e.Title,
                    e.Start,
                    e.End,
                    e.Cabinet?.Name)));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventAsync(Guid id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var resEvent = await _dbContext.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (resEvent == null)
                return NotFound();

            if (!resEvent.IsPublic && !resEvent.Listeners.Contains(currentUser) && resEvent.Creator.Id != currentUser.Id)
                return Forbid();

            return Ok(new EventDto(
                resEvent.Id,
                resEvent.Title,
                resEvent.Start,
                resEvent.End,
                resEvent.Description,
                resEvent.Cabinet?.Name,
                resEvent.IsPublic,
                new UserHeaderDto(
                    resEvent.Creator.Id,
                    resEvent.Creator.FirstName,
                    resEvent.Creator.Surname,
                    resEvent.Creator.Patronymic,
                    resEvent?.Creator?.Image?.Id),
                resEvent.Listeners
                    .Select(u => new UserHeaderDto(
                        u.Id,
                        u.FirstName,
                        u.Surname,
                        u.Patronymic,
                        u?.Image?.Id))));
        }

        [HttpPost]
        public async Task<IActionResult> PostEventAsync([FromBody] PostEventRequest request)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var newEvent = new Event
            {
                Title = request.Title,
                Start = request.Start,
                End = request.End,
                Description = request.Description,
                Creator = currentUser
            };

            if (request.CabinetId != null)
            {
                if (!User.IsInRole(RolesNames.CabinetsManager))
                    return Forbid();

                if (request.Start > request.End)
                    return BadRequest();

                var cabinet = await _dbContext.Cabinets.FirstOrDefaultAsync(c => c.Id == request.CabinetId);
                if (cabinet == null)
                    return BadRequest();

                if (cabinet.Events
                    .Where(e => request.Start < e.Start && request.End > e.Start
                        || request.Start > e.Start && request.End < e.End).Any())
                    return BadRequest();

                newEvent.Cabinet = cabinet;
            }

            if (request.IsPublic)
            {
                if (!User.IsInRole(RolesNames.Support))
                    return Forbid();
                newEvent.IsPublic = true;
            }

            if (request.ListenersIds != null && request.ListenersIds.Any())
                if (!request.IsPublic)
                    foreach (var userId in request.ListenersIds)
                    {
                        var user = await _userManager.FindByIdAsync(userId);
                        if (user == null)
                            return BadRequest();
                    }
                else
                    return BadRequest();

            await _dbContext.AddAsync(newEvent);
            await _dbContext.SaveChangesAsync();

            return Ok(newEvent.Id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEventAsync(Guid id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var resEvent = await _dbContext.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (resEvent == null)
                return NotFound();

            if (!(resEvent.IsPublic && User.IsInRole(RolesNames.Support) || resEvent.Creator.Id == currentUser.Id))
                return Forbid();

            _dbContext.Events.Remove(resEvent);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
