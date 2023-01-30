﻿using IrzUccApi.Models.Db;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.PagingOptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Controllers.News
{
    [Route("api/likes")]
    [ApiController]
    [Authorize]
    public class NewsLikesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public NewsLikesController(AppDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpPost("like_news_entry")]
        public async Task<IActionResult> LikeNewsEntry([Required] int newsEntryId)
            => await LikeUnlikeNewsEntry(newsEntryId, true);

        [HttpPost("unlike_news_entry")]
        public async Task<IActionResult> UnlikeNewsEntry([Required] int newsEntryId)
            => await LikeUnlikeNewsEntry(newsEntryId, false);

        private async Task<IActionResult> LikeUnlikeNewsEntry(int newsEntryId, bool isLike)
        {
            var newsEntry = _dbContext.NewsEntries.FirstOrDefault(n => n.Id == newsEntryId);
            if (newsEntry == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            if (isLike)
            {
                if (!newsEntry.Likers.Contains(currentUser))
                    newsEntry.Likers.Add(currentUser);
            }
            else
            {
                if (newsEntry.Likers.Contains(currentUser))
                    newsEntry.Likers.Remove(currentUser);
            }

            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}