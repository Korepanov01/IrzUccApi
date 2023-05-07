using IrzUccApi.Db.Models;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.PagingOptions;
using Microsoft.EntityFrameworkCore;

namespace IrzUccApi.Db.Repositories
{
    public class CommentRepository : AppRepository<Comment, AppDbContext>
    {
        public CommentRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<CommentDto>> GetCommentDtosAsync(Guid newsEntrId, PagingParameters parameters)
            => await _dbContext.Comments
            .Where(c => c.NewsEntry.Id == newsEntrId)
            .OrderByDescending(c => c.DateTime)
            .Skip(parameters.PageSize * (parameters.PageIndex - 1))
            .Take(parameters.PageSize)
            .Select(c => new CommentDto(
                c.Id,
                c.Text,
                c.DateTime,
                new UserHeaderDto(
                    c.Author.Id,
                    c.Author.FirstName,
                    c.Author.Surname,
                    c.Author.Patronymic,
                    c.Author.Image != null ? c.Author.Image.Id : null)))
            .ToArrayAsync();
    }
}
