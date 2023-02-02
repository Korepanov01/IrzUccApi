using IrzUccApi.Enums;
using IrzUccApi.Models.Db;
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

        [HttpPost]
        public async Task<IActionResult> PostEvent([FromBody] PostEventRequest request)
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
    }
}
