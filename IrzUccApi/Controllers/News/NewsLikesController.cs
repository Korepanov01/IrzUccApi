using IrzUccApi.Db;
using IrzUccApi.Db.Models;
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
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public NewsLikesController(UserManager<AppUser> userManager, UnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("like_news_entry")]
        public async Task<IActionResult> LikeNewsEntryAsync([Required] Guid newsEntryId)
            => await LikeUnlikeNewsEntryAsync(newsEntryId, true);

        [HttpPost("unlike_news_entry")]
        public async Task<IActionResult> UnlikeNewsEntryAsync([Required] Guid newsEntryId)
            => await LikeUnlikeNewsEntryAsync(newsEntryId, false);

        private async Task<IActionResult> LikeUnlikeNewsEntryAsync(Guid newsEntryId, bool isLike)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var newsEntry = await _unitOfWork.NewsEntries.GetByIdAsync(newsEntryId);
            if (newsEntry == null)
                return NotFound();

            if (isLike)
            {
                newsEntry.Likers.Add(currentUser);
            }
            else
            {
                newsEntry.Likers.Remove(currentUser);
            }

            _unitOfWork.NewsEntries.Update(newsEntry);
            await _unitOfWork.SaveAsync();

            return Ok();
        }
    }
}
