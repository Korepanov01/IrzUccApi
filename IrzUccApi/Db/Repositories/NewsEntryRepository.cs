using IrzUccApi.Db.Models;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.GetOptions;
using Microsoft.EntityFrameworkCore;

namespace IrzUccApi.Db.Repositories
{
    public class NewsEntryRepository : AppRepository<NewsEntry, AppDbContext>
    {
        public NewsEntryRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<NewsEntryDto>> GetNewsEntryDtosAsync(AppUser? currentUser, NewsSearchParameters parameters)
        {
            var news = _dbContext.NewsEntries.AsQueryable();

            if (currentUser != null && parameters.AuthorId != null)
                news = news.Where(n => n.Author.Id == parameters.AuthorId);
            if (currentUser == null || parameters.PublicOnly)
                news = news.Where(n => n.IsPublic);
            if (currentUser != null && parameters.AuthorId == null)
                news = news.Where(n => n.IsPublic || currentUser.Subscriptions.Contains(n.Author) || n.Author.Id == currentUser.Id);
            if (currentUser != null && parameters.LikedOnly)
                news = news.Where(n => n.Likers.Contains(currentUser));

            if (parameters.SearchString != null)
            {
                var normalizedSearchWords = parameters.SearchString
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(sw => sw.ToUpper());
                foreach (var word in normalizedSearchWords)
                    news = news.Where(n => (n.Title + n.Text).ToUpper().Contains(word));
            }

            return await news
                .OrderByDescending(n => n.DateTime)
                .Skip(parameters.PageSize * (parameters.PageIndex - 1))
                .Take(parameters.PageSize)
                .Select(n => new NewsEntryDto(
                    n.Id,
                    n.Title,
                    n.Text.Substring(0, Math.Min(200, n.Text.Length)),
                    n.Image != null ? n.Image.Id : null,
                    n.DateTime,
                    currentUser != null && currentUser.LikedNewsEntries.Contains(n),
                    n.Likers.Count,
                    new UserHeaderDto(
                        n.Author.Id,
                        n.Author.FirstName,
                        n.Author.Surname,
                        n.Author.Patronymic,
                        n.Author.Image != null ? n.Author.Image.Id : null),
                    n.IsPublic,
                    n.Comments.Count,
                    n.Text.Length > 200))
                .ToArrayAsync();
        }
    }
}
