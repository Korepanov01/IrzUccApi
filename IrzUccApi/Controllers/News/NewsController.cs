using IrzUccApi.Enums;
using IrzUccApi.Models.Db;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.GetOptions;
using IrzUccApi.Models.Requests.News;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace IrzUccApi.Controllers.News
{
    [Route("api/news")]
    [ApiController]
    [Authorize]
    public class NewsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public NewsController(AppDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetNews([FromQuery] NewsSearchParameters parameters)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var news = _dbContext.NewsEntries.AsQueryable();

            if (currentUser != null && parameters.AuthorId != null)
                news = news.Where(n => n.Author.Id == parameters.AuthorId);
            if (currentUser == null || parameters.PublicOnly)
                news = news.Where(n => n.IsPublic);
            if (currentUser != null && parameters.AuthorId == null)
                news = news.Where(n => n.IsPublic || currentUser.Subscriptions.Contains(n.Author));
            if (currentUser != null && parameters.LikedOnly)
                news = news.Where(n => n.Likers.Contains(currentUser));

            if (parameters.SearchString != null)
            {
                var normalizedSearchString = parameters.SearchString.ToUpper();
                news = news.Where(n => (n.Title + n.Text).ToUpper().Contains(normalizedSearchString));
            }

            return Ok(await news
                .OrderBy(n => n.DateTime)
                .Skip(parameters.PageSize * (parameters.PageIndex - 1))
                .Take(parameters.PageSize)
                .Select(n => new NewsEntryDto(
                    n.Id,
                    n.Title,
                    n.Text.Substring(0, Math.Min(100, n.Text.Length)),
                    n.Image,
                    n.DateTime,
                    currentUser != null && currentUser.LikedNewsEntries.Contains(n),
                    n.Likers.Count,
                    new UserHeaderDto(
                        n.Author.Id,
                        n.Author.FirstName,
                        n.Author.Surname,
                        n.Author.Patronymic,
                        n.Author.Image),
                    n.IsPublic,
                    n.Comments.Count))
                .ToArrayAsync());
        }

        [HttpPost]
        public async Task<IActionResult> PostNewsEntry([FromBody] PostNewsEntryRequest request)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            if (request.IsPublic && !User.IsInRole(RolesNames.Publisher))
                return Forbid();

            var newsEntry = new NewsEntry
            {
                Title = request.Title,
                Text = request.Text,
                Image = request.Image,
                DateTime = DateTime.UtcNow,
                Author = currentUser,
                IsPublic = request.IsPublic
            };

            await _dbContext.AddAsync(newsEntry);

            await _dbContext.SaveChangesAsync();

            return Ok(newsEntry.Id);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNewsEntry(int id)
        {
            var newsEntry = _dbContext.NewsEntries.FirstOrDefault(n => n.Id == id);
            if (newsEntry == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (!newsEntry.IsPublic && currentUser == null)
                return Forbid();

            return Ok(new NewsEntryDto(
                    newsEntry.Id,
                    newsEntry.Title,
                    newsEntry.Text,
                    newsEntry.Image,
                    newsEntry.DateTime,
                    currentUser != null && currentUser.LikedNewsEntries.Contains(newsEntry),
                    newsEntry.Likers.Count,
                    new UserHeaderDto(
                        newsEntry.Author.Id,
                        newsEntry.Author.FirstName,
                        newsEntry.Author.Surname,
                        newsEntry.Author.Patronymic,
                        newsEntry.Author.Image),
                    newsEntry.IsPublic,
                    newsEntry.Comments.Count));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNewsEntry(int id)
        {
            var newsEntry = _dbContext.NewsEntries.FirstOrDefault(n => n.Id == id);
            if (newsEntry == null)
                return NotFound();

            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null)
                return Unauthorized();
            if (newsEntry.Author.Id != currentUserId && !newsEntry.IsPublic || !User.IsInRole(RolesNames.Publisher) && newsEntry.IsPublic)
                return Forbid();

            _dbContext.Remove(newsEntry);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
