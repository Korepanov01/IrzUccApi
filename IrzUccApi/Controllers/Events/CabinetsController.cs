﻿using IrzUccApi.Db;
using IrzUccApi.Db.Models;
using IrzUccApi.Enums;
using IrzUccApi.ErrorDescribers;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.GetOptions;
using IrzUccApi.Models.PagingOptions;
using IrzUccApi.Models.Requests.Events;
using Microsoft.AspNetCore.Authorization;
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

        public CabinetsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetCabinetsAsync([FromQuery] CabinetsGetParameters parameters)
        {
            var cabinets = _dbContext.Cabinets.AsQueryable();

            if (parameters.SearchString != null)
            {
                var normalizedSearchWords = parameters.SearchString
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(sw => sw.ToUpper());
                foreach (var word in normalizedSearchWords)
                    cabinets = cabinets.Where(c => c.Name.ToUpper().Contains(word));
            }

            if (parameters.TimeRange != null)
            {
                var start = parameters.TimeRange.Start.ToUniversalTime();
                var end = parameters.TimeRange.End.ToUniversalTime();

                if (start > end)
                    return BadRequest(new[] { RequestErrorDescriber.EndTimeIsLessThenStartTime });

                cabinets = cabinets.Where(c => !c.Events.Where(e =>
                    start < e.Start && end > e.Start || start > e.Start && end < e.End).Any());
            }

            return Ok(await cabinets
                .OrderBy(c => c.Name)
                .Skip(parameters.PageSize * parameters.PageIndex)
                .Take(parameters.PageSize)
                .Select(c => new CabinetDto(
                    c.Id,
                    c.Name))
                .ToArrayAsync());
        }

        [HttpGet("{id}/events")]
        public async Task<IActionResult> GetCabinetEventsAsync(Guid id, [FromQuery] PagingParameters parameters)
        {
            var cabinet = await _dbContext.Cabinets.FirstOrDefaultAsync(c => c.Id == id);
            if (cabinet == null)
                return NotFound();

            return Ok(cabinet.Events
                .OrderBy(e => e.Start)
                .Skip(parameters.PageSize * parameters.PageIndex)
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
        public async Task<IActionResult> PostCabinetAsync([FromBody] PostPutCabinetRequest request)
        {
            if (await _dbContext.Cabinets.FirstOrDefaultAsync(c => c.Name == request.Name) != null)
                return BadRequest(new[] { RequestErrorDescriber.CabinetAlreadyExists });

            var cabinet = new Cabinet
            {
                Name = request.Name
            };

            await _dbContext.AddAsync(cabinet);
            await _dbContext.SaveChangesAsync();

            return Ok(cabinet.Id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCabinetAsync(Guid id, [FromBody] PostPutCabinetRequest request)
        {
            var cabinet = await _dbContext.Cabinets.FirstOrDefaultAsync(c => c.Id == id);
            if (cabinet == null)
                return NotFound();

            if (await _dbContext.Cabinets.FirstOrDefaultAsync(c => c.Name == request.Name) != null)
                return BadRequest(new[] { RequestErrorDescriber.CabinetAlreadyExists });

            cabinet.Name = request.Name;
            _dbContext.Update(cabinet);

            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCabinetAsync(Guid id)
        {
            var cabinet = await _dbContext.Cabinets.FirstOrDefaultAsync(c => c.Id == id);
            if (cabinet == null)
                return NotFound();

            if (cabinet.Events.Count != 0)
                return BadRequest(new[] { RequestErrorDescriber.CabinetIsBooked });

            _dbContext.Cabinets.Remove(cabinet);

            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
