using IrzUccApi.Db.Models;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.PagingOptions;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace IrzUccApi.Db.Repositories
{
    public class EventRepository : AppRepository<Event, AppDbContext>
    {
        public EventRepository(AppDbContext dbContext) : base(dbContext) { }

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
