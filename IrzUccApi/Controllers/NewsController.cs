using IrzUccApi.Enums;
using IrzUccApi.Models;
using IrzUccApi.Models.Dtos.NewsEntry;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

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

        [HttpGet("feed")]
        [Authorize]
        public async Task<IActionResult> GetNews(
            [Range(0, 50)] int pageSize = 10,
            [Range(1, int.MaxValue)] int page = 1
            )
        {
            var currentUser = await _userIdentifier.GetCurrentUser(User);
            if (currentUser == null)
                return Unauthorized();

            var subscribtionsIds = currentUser.Subscriptions.Select(s => s.Id);

            var news = await _dbContext.NewsEntries
                .Where(n => subscribtionsIds.Contains(n.Author.Id))
                .OrderBy(n => n.DateTime)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(n => BuildNewsEntryDto(n, null, true))
                .ToArrayAsync();
            return Ok(news);
        }

        [HttpGet("public")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublicNews(
            [Range(0, 50)] int pageSize = 10,
            [Range(1, int.MaxValue)] int page = 1
            )
        {
            return Ok(await _dbContext.NewsEntries
                .Where(n => n.IsPublic)
                .OrderBy(n => n.DateTime)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(n => BuildNewsEntryDto(n, null, true))
                .ToArrayAsync());
        }

        [HttpGet("user/{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserNews(
            string userId,
            [Range(0, 50)] int pageSize = 10,
            [Range(1, int.MaxValue)] int page = 1
            )
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var currentUser = await _userIdentifier.GetCurrentUser(User);
            if (currentUser == null)
                return Unauthorized();

            return Ok(user.NewsEntries
                .OrderBy(n => n.DateTime)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(n => BuildNewsEntryDto(n, currentUser, true))
                .ToArray());
        }

        [HttpGet("liked")]
        [Authorize]
        public async Task<IActionResult> GetLikedNews(
            [Range(0, 50)] int pageSize = 10,
            [Range(1, int.MaxValue)] int page = 1
            )
        {
            var currentUser = await _userIdentifier.GetCurrentUser(User);
            if (currentUser == null) return Unauthorized();
            return Ok(currentUser.LikedNewsEntries
                .OrderBy(n => n.DateTime)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(n => BuildNewsEntryDto(n, currentUser, true))
                .ToArray());
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
                IsPublic = request.IsPublic && User.IsInRole(Roles.Publisher)
            });
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("{id}/like_unlike")]
        [Authorize]
        public async Task<IActionResult> LikeUnlikeNewsEntry(int id)
        {
            var newsEntry = _dbContext.NewsEntries.FirstOrDefault(n => n.Id == id);
            if (newsEntry == null)
                return NotFound();

            var currentUser = (await _userIdentifier.GetCurrentUser(User)) ?? throw new Exception();
            if (newsEntry.Likers.Contains(currentUser))
                newsEntry.Likers.Remove(currentUser);
            else
                newsEntry.Likers.Add(currentUser);

            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNewsEntry(int id)
        {
            var newsEntry = _dbContext.NewsEntries.FirstOrDefault(n => n.Id == id);
            if (newsEntry == null)
                return NotFound();

            var currentUser = await _userIdentifier.GetCurrentUser(User);
            if (!newsEntry.IsPublic && currentUser == null)
                return Forbid();

            return Ok(BuildNewsEntryDto(newsEntry, null, false));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNewsEntry(int id)
        {
            var newsEntry = _dbContext.NewsEntries.FirstOrDefault(n => n.Id == id);
            if (newsEntry == null)
                return NotFound();

            var currentUserId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            if (newsEntry.IsPublic && !User.IsInRole(Roles.Publisher) || newsEntry.Author.Id != currentUserId)
                return Forbid();

            _dbContext.Remove(newsEntry);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{id}/sub_unsub")]
        [Authorize]
        public async Task<IActionResult> SubUnsub([FromBody][Required] string userId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound();

            var currentUser = (await _userIdentifier.GetCurrentUser(User)) ?? throw new Exception();

            if (user.Subscribers.Contains(currentUser))
                user.Subscribers.Remove(currentUser);
            else
                user.Subscribers.Add(currentUser);

            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        private NewsEntryDto BuildNewsEntryDto(NewsEntry newsEntry, AppUser? currentUser, bool isShort)
        {
            return new NewsEntryDto(
                    newsEntry.Id,
                    newsEntry.Title,
                    isShort ? newsEntry.Text[..Math.Min(100, newsEntry.Text.Length)] : newsEntry.Text,
                    newsEntry.Image,
                    newsEntry.DateTime,
                    currentUser != null && currentUser.LikedNewsEntries.Contains(newsEntry),
                    newsEntry.Likers.Count,
                    newsEntry.Author.Id,
                    newsEntry.Author.SurnameFirstName,
                    newsEntry.IsPublic);
        }
    }
}
