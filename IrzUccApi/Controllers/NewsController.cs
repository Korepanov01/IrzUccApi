using IrzUccApi.Models;
using IrzUccApi.Models.Dtos.NewsEntry;
using IrzUccApi.Models.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace IrzUccApi.Controllers
{
    [Route("api/news")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly UserIdentifier _userIdentifier;

        public NewsController(AppDbContext dbContext, UserManager<AppUser> userManager, UserIdentifier userIdentifier)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _userIdentifier = userIdentifier;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetNews(
            int pageSize = 10,
            int page = 1
            )
        {
            var currentUser = await _userIdentifier.GetCurrentUser(User);
            IEnumerable<string> subscriptionsIds = currentUser?.Subscriptions?.Select(s => s.Id) ?? Array.Empty<string>();
            var news = await _dbContext.NewsEntries
                .Where(n => n.IsPublic || subscriptionsIds.Contains(n.Author.Id) && n.Author.IsActiveAccount)
                .OrderBy(n => n.DateTime)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(n => CalcNewsEntryDto(n, currentUser, true))
                .ToArrayAsync();
            return Ok(news);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostNewsEntry([FromBody] PostNewsEntryRequest request)
        {
            await _dbContext.AddAsync(new NewsEntry
            {
                Title = request.Title,
                Text = request.Text,
                Image = request.Image,
                DateTime = DateTime.UtcNow,
                Author = await _userIdentifier.GetCurrentUser(User) ?? throw new Exception(),
                IsPublic = request.IsPublic && User.IsInRole("Publisher")
            });
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNewsEntry(int id)
        {
            var newsEntry = _dbContext.NewsEntries.FirstOrDefault(n => n.Id == id);
            if (newsEntry == null)
                return NotFound();

            return Ok(CalcNewsEntryDto(newsEntry, await _userIdentifier.GetCurrentUser(User), false));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNewsEntry(int id)
        {
            var newsEntry = _dbContext.NewsEntries.FirstOrDefault(n => n.Id == id);
            if (newsEntry == null)
                return NotFound();

            var currentUserId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            if (newsEntry.IsPublic && !User.IsInRole("Publisher") && newsEntry.Author.Id != currentUserId)
                return Forbid();

            _dbContext.Remove(newsEntry);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        private static NewsEntryDto CalcNewsEntryDto(NewsEntry newsEntry, AppUser? currentUser, bool isShort)
        {
            return new NewsEntryDto
            {
                Id = newsEntry.Id,
                Title = newsEntry.Title,
                Text = isShort ? newsEntry.Text[..Math.Min(newsEntry.Text.Length, 100)] : newsEntry.Text,
                Image = newsEntry.Image,
                DateTime = newsEntry.DateTime,
                IsLiked = currentUser != null && newsEntry.Likers.Contains(currentUser),
                LikesCount = newsEntry.Likers.Count,
                AuthorId = newsEntry.Author.Id,
                AuthorName = newsEntry.Author.Surname + " " + newsEntry.Author.FirstName,
                IsPublic= newsEntry.IsPublic
            };
        }
    }
}
