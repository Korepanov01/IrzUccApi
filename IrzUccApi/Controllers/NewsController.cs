using IrzUccApi.Enums;
using IrzUccApi.Models.Db;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.Requests.News;
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

        [HttpGet("feed")]
        [Authorize]
        public async Task<IActionResult> GetNews(
            [Range(0, 50)] int pageSize = 10,
            [Range(1, int.MaxValue)] int page = 1
            )
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var subscribtionsIds = currentUser.Subscriptions.Select(s => s.Id);

            var news = await _dbContext.NewsEntries
                .Where(n => n.IsPublic || subscribtionsIds.Contains(n.Author.Id))
                .OrderBy(n => n.DateTime)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
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
                        n.Author.Email,
                        n.Author.Image),
                    n.IsPublic))
                .ToArrayAsync();
            return Ok(news);
        }

        [HttpGet("public")]
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
                .Select(n => new NewsEntryDto(
                    n.Id,
                    n.Title,
                    n.Text.Substring(0, Math.Min(100, n.Text.Length)),
                    n.Image,
                    n.DateTime,
                    false,
                    n.Likers.Count,
                    new UserHeaderDto(
                        n.Author.Id,
                        n.Author.FirstName,
                        n.Author.Surname,
                        n.Author.Patronymic,
                        n.Author.Email,
                        n.Author.Image),
                    n.IsPublic))
                .ToArrayAsync());
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserNews(
            string id,
            [Range(0, 50)] int pageSize = 10,
            [Range(1, int.MaxValue)] int page = 1
            )
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            return Ok(user.NewsEntries
                .OrderBy(n => n.DateTime)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
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
                        n.Author.Email,
                        n.Author.Image),
                    n.IsPublic))
                .ToArray());
        }

        [HttpPost]
        public async Task<IActionResult> PostNewsEntry([FromBody] PostNewsEntryRequest request)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            if (request.IsPublic && !User.IsInRole(RolesNames.Publisher))
                return Forbid();

            await _dbContext.AddAsync(new NewsEntry
            {
                Title = request.Title,
                Text = request.Text,
                Image = request.Image,
                DateTime = DateTime.UtcNow,
                Author = currentUser,
                IsPublic = request.IsPublic
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

            var currentUser = await _userManager.GetUserAsync(User);
            if (!newsEntry.IsPublic && currentUser == null)
                return Forbid();

            return Ok(new NewsEntryDto(
                    newsEntry.Id,
                    newsEntry.Title,
                    newsEntry.Text.Substring(0, Math.Min(100, newsEntry.Text.Length)),
                    newsEntry.Image,
                    newsEntry.DateTime,
                    currentUser != null && currentUser.LikedNewsEntries.Contains(newsEntry),
                    newsEntry.Likers.Count,
                    new UserHeaderDto(
                        newsEntry.Author.Id,
                        newsEntry.Author.FirstName,
                        newsEntry.Author.Surname,
                        newsEntry.Author.Patronymic,
                        newsEntry.Author.Email,
                        newsEntry.Author.Image),
                    newsEntry.IsPublic));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNewsEntry(int id)
        {
            var newsEntry = _dbContext.NewsEntries.FirstOrDefault(n => n.Id == id);
            if (newsEntry == null)
                return NotFound();

            var currentUserId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            if (newsEntry.Author.Id != currentUserId && !newsEntry.IsPublic || !User.IsInRole(RolesNames.Publisher) && newsEntry.IsPublic)
                return Forbid();

            _dbContext.Remove(newsEntry);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
