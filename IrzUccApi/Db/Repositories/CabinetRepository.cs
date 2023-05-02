using IrzUccApi.Db.Models;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.GetOptions;
using IrzUccApi.Models.PagingOptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrzUccApi.Db.Repositories
{
    public class CabinetRepository : AppRepository<Cabinet, AppDbContext>
    {
        public CabinetRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<CabinetDto>> GetAsync(CabinetsGetParameters parameters)
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

                cabinets = cabinets.Where(c => !c.Events.Where(e =>
                    start < e.Start && end > e.Start || start > e.Start && end < e.End).Any());
            }

            return await cabinets
                .OrderBy(c => c.Name)
                .Skip(parameters.PageSize * (parameters.PageIndex - 1))
                .Take(parameters.PageSize)
                .Select(c => new CabinetDto(
                    c.Id,
                    c.Name))
                .ToArrayAsync();
        }

        public async Task<bool> ExistsAsync(string name)
            => await ExistsAsync(c => c.Name == name);

        public async Task<bool> IsBookedAsync(Cabinet cabinet)
            => await _dbContext.Events.Where(e => e.Cabinet != null && e.Id == cabinet.Id).AnyAsync();
    }
}
