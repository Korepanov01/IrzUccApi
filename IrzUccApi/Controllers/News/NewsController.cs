﻿using IrzUccApi.Db;
using IrzUccApi.Enums;
using IrzUccApi.Models.Db;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.GetOptions;
using IrzUccApi.Models.Requests.News;
using IrzUccApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
                news = news.Where(n => n.IsPublic || currentUser.Subscriptions.Contains(n.Author) || n.Author.Id == currentUser.Id);
            if (currentUser != null && parameters.LikedOnly)
                news = news.Where(n => n.Likers.Contains(currentUser));

            if (parameters.SearchString != null)
            {
                var normalizedSearchString = parameters.SearchString.ToUpper();
                news = news.Where(n => (n.Title + n.Text).ToUpper().Contains(normalizedSearchString));
            }

            return Ok(await news
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
                .ToArrayAsync());
        }

        [HttpPost]
        public async Task<IActionResult> PostNewsEntry([FromBody] PostNewsEntryRequest request)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            if (request.IsPublic && !User.IsInRole(RolesNames.Support))
                return Forbid();

            var newNewsEntryId = Guid.NewGuid();

            Image? image = null;
            if (request.Image != null)
            {
                image = new Image
                {
                    Id = Guid.NewGuid(),
                    Name = request.Image.Name,
                    Extension = request.Image.Extension,
                    Data = request.Image.Data,
                    Source = ImageSources.NewsEntry,
                    SourceId = newNewsEntryId
                };
                await _dbContext.Images.AddAsync(image);
            }

            var newsEntry = new NewsEntry
            {
                Id = newNewsEntryId,
                Title = request.Title,
                Text = request.Text,
                Image = image,
                DateTime = DateTime.UtcNow,
                Author = currentUser,
                IsPublic = request.IsPublic
            };
            await _dbContext.AddAsync(newsEntry);

            await _dbContext.SaveChangesAsync();

            return Ok(newsEntry.Id);
        }

        [HttpGet("{id}/full_text")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNewsEntryText(Guid id)
        {
            var newsEntry = _dbContext.NewsEntries.FirstOrDefault(n => n.Id == id);
            if (newsEntry == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (!newsEntry.IsPublic && currentUser == null)
                return Forbid();

            return Ok(newsEntry.Text);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNewsEntry(Guid id)
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
                    newsEntry.Image?.Id,
                    newsEntry.DateTime,
                    currentUser != null && currentUser.LikedNewsEntries.Contains(newsEntry),
                    newsEntry.Likers.Count,
                    new UserHeaderDto(
                        newsEntry.Author.Id,
                        newsEntry.Author.FirstName,
                        newsEntry.Author.Surname,
                        newsEntry.Author.Patronymic,
                        newsEntry.Author.Image?.Id),
                    newsEntry.IsPublic,
                    newsEntry.Comments.Count,
                    false));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNewsEntry(Guid id)
        {
            var newsEntry = _dbContext.NewsEntries.FirstOrDefault(n => n.Id == id);
            if (newsEntry == null)
                return NotFound();

            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null)
                return Unauthorized();
            if (newsEntry.Author.Id != new Guid(currentUserId) && !newsEntry.IsPublic || !User.IsInRole(RolesNames.Support) && newsEntry.IsPublic)
                return Forbid();

            _dbContext.Remove(newsEntry);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
