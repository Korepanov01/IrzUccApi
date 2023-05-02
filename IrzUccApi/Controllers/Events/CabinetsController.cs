using IrzUccApi.Db;
using IrzUccApi.Db.Models;
using IrzUccApi.Enums;
using IrzUccApi.ErrorDescribers;
using IrzUccApi.Models.GetOptions;
using IrzUccApi.Models.PagingOptions;
using IrzUccApi.Models.Requests.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IrzUccApi.Controllers.Events
{
    [Route("api/cabinets")]
    [ApiController]
    [Authorize(Roles = RolesNames.CabinetsManager)]
    public class CabinetsController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public CabinetsController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetCabinetsAsync([FromQuery] CabinetsGetParameters parameters)
        {
            if (parameters.TimeRange != null && parameters.TimeRange.Start > parameters.TimeRange.End)
                return BadRequest(new[] { RequestErrorDescriber.EndTimeIsLessThenStartTime });

            var cabinets = await _unitOfWork.Cabinets.GetAsync(parameters);

            return Ok(cabinets);
        }

        [HttpGet("{id}/events")]
        public async Task<IActionResult> GetCabinetEventsAsync(Guid id, [FromQuery] PagingParameters parameters)
        {
            var cabinet = await _unitOfWork.Cabinets.GetByIdAsync(id);
            if (cabinet == null)
                return NotFound();

            var events = await _unitOfWork.Events.GetByCabinetAsync(cabinet, parameters);

            return Ok(events);
        }

        [HttpPost]
        public async Task<IActionResult> PostCabinetAsync([FromBody] PostPutCabinetRequest request)
        {
            if (await _unitOfWork.Cabinets.ExistsAsync(request.Name))
                return BadRequest(new[] { RequestErrorDescriber.CabinetAlreadyExists });
                
            var cabinet = new Cabinet
            {
                Name = request.Name
            };

            await _unitOfWork.Cabinets.AddAsync(cabinet);

            return Ok(cabinet.Id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCabinetAsync(Guid id, [FromBody] PostPutCabinetRequest request)
        {
            var cabinet = await _unitOfWork.Cabinets.GetByIdAsync(id);
            if (cabinet == null)
                return NotFound();

            if (await _unitOfWork.Cabinets.ExistsAsync(request.Name))
                return BadRequest(new[] { RequestErrorDescriber.CabinetAlreadyExists });

            cabinet.Name = request.Name;

            await _unitOfWork.Cabinets.UpdateAsync(cabinet);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCabinetAsync(Guid id)
        {
            var cabinet = await _unitOfWork.Cabinets.GetByIdAsync(id);
            if (cabinet == null)
                return NotFound();

            if (await _unitOfWork.Cabinets.IsBookedAsync(cabinet))
                return BadRequest(new[] { RequestErrorDescriber.CabinetIsBooked });

            await _unitOfWork.Cabinets.DeleteAsync(cabinet);

            return Ok();
        }
    }
}
