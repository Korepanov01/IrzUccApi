using IrzUccApi.Db.Models;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.GetOptions;
using IrzUccApi.Models.PagingOptions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace IrzUccApi.Db.Repositories
{
    public class EventRepository : AppRepository<Event, AppDbContext>
    {
        public EventRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<EventListItemDto>> GetByListenerAsync(AppUser listener, TimeRangeGetParameters parameters)
        {
            var start = parameters.Start.ToUniversalTime();
            var end = parameters.End.ToUniversalTime();

            var events = await _dbContext.Events
                .OrderBy(e => e.Start)
                .Where(e => e.IsPublic || e.Listeners.Contains(listener) || e.Creator.Id == listener.Id)
                .Where(e => start < e.Start && end > e.Start
                    || start > e.Start && end < e.End)
                .Select(e => new EventListItemDto(
                    e.Id,
                    e.Title,
                    e.Start,
                    e.End,
                    e.Cabinet != null ? e.Cabinet.Name : null))
                .ToArrayAsync();

            return events;
        }

        public async Task<IEnumerable<EventListItemDto>> GetByCreatorIdAsync(Guid userId, PagingParameters parameters)
        {
            var events = await _dbContext.Events
                .Where(e => e.Creator.Id == userId)
                .OrderBy(e => e.Start)
                .Skip(parameters.PageSize * (parameters.PageIndex - 1))
                .Take(parameters.PageSize)
                .Select(e => new EventListItemDto(
                    e.Id,
                    e.Title,
                    e.Start,
                    e.End,
                    e.Cabinet != null ? e.Cabinet.Name : null))
                .ToArrayAsync();

            return events;
        }

        public async Task<IEnumerable<EventListItemDto>> GetByCabinetAsync(Cabinet cabinet, PagingParameters parameters)
        {
            var events = await _dbContext.Events
                .Where(e => e.Cabinet != null && e.Cabinet.Id == cabinet.Id)
                .OrderBy(e => e.Start)
                .Skip(parameters.PageSize * (parameters.PageIndex - 1))
                .Take(parameters.PageSize)
                .Select(e => new EventListItemDto(
                    e.Id,
                    e.Title,
                    e.Start,
                    e.End,
                    cabinet.Name))
                .ToArrayAsync();

            return events;
        }
    }
}
