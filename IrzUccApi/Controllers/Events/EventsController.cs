using IrzUccApi.Db;
using IrzUccApi.Db.Models;
using IrzUccApi.Enums;
using IrzUccApi.ErrorDescribers;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.GetOptions;
using IrzUccApi.Models.PagingOptions;
using IrzUccApi.Models.Requests.Events;
using IrzUccApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IrzUccApi.Controllers.Events
{
    [Route("api/events")]
    [ApiController]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public EventsController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyEventsAsync([FromQuery] PagingParameters parameters)
        {
            var currentUserId = ClaimsExtractor.GetNameIdentifier(User);
            if (currentUserId == null)
                return Unauthorized();

            var userEvent = await _unitOfWork.Events.GetByCreatorIdAsync(Guid.Parse(currentUserId), parameters);

            return Ok(userEvent);
        }

        [HttpGet("listenning")]
        public async Task<IActionResult> GetListenningEventsAsync([FromQuery] TimeRangeGetParameters parameters)
        {
            var currentUser = await _unitOfWork.Users.GetByClaimsAsync(User);
            if (currentUser == null)
                return Unauthorized();

            if (parameters.Start > parameters.End)
                return BadRequest(new[] { RequestErrorDescriber.EndTimeIsLessThenStartTime });
            if ((parameters.End - parameters.Start).Days > 40)
                return BadRequest(new[] { RequestErrorDescriber.TooLongPeriod });

            var events = await _unitOfWork.Events.GetByListenerAsync(currentUser, parameters);

            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventAsync(Guid id)
        {
            var currentUser = await _unitOfWork.Users.GetByClaimsAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var resEvent = await _unitOfWork.Events.GetByIdAsync(id);
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
                resEvent!.Listeners
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
            var currentUser = await _unitOfWork.Users.GetByClaimsAsync(User);
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
                    return BadRequest(new[] { RequestErrorDescriber.EndTimeIsLessThenStartTime });

                var cabinet = await _unitOfWork.Cabinets.GetByIdAsync((Guid)request.CabinetId);
                if (cabinet == null)
                    return BadRequest(new[] { RequestErrorDescriber.CabinetNotFound });

                if (await _unitOfWork.Cabinets.IsBookedAsync(cabinet, request.End, request.Start))
                    return BadRequest(new[] { RequestErrorDescriber.CabinetIsBooked });

                newEvent.Cabinet = cabinet;
            }

            if (request.IsPublic)
            {
                if (!User.IsInRole(RolesNames.Support))
                    return Forbid();
                newEvent.IsPublic = true;
            }

            if (request.ListenersIds != null)
            {
                request.ListenersIds.Remove(currentUser.Id);

                if (request.ListenersIds.Any())
                {
                    if (request.IsPublic)
                        return BadRequest(new[] { RequestErrorDescriber.PublicEventHasNotListeners });

                    foreach (var userId in request.ListenersIds)
                    {
                        var user = await _unitOfWork.Users.GetByIdAsync(userId);
                        if (user == null)
                            return BadRequest(new[] { RequestErrorDescriber.UserDoesntExist });
                        newEvent.Listeners.Add(user);
                    }
                }
            }

            await _unitOfWork.Events.AddAsync(newEvent);

            return Ok(newEvent.Id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEventAsync(Guid id)
        {
            var currentUserId = ClaimsExtractor.GetNameIdentifier(User);
            if (currentUserId == null)
                return Unauthorized();

            var resEvent = await _unitOfWork.Events.GetByIdAsync(id);
            if (resEvent == null)
                return NotFound();

            if (!(resEvent.IsPublic && User.IsInRole(RolesNames.Support) || resEvent.Creator.Id.ToString() == currentUserId))
                return Forbid();

            await _unitOfWork.Events.DeleteAsync(resEvent);

            return Ok();
        }
    }
}
