using IrzUccApi.Db;
using IrzUccApi.Db.Models;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.PagingOptions;
using IrzUccApi.Models.Requests.News;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Controllers.News
{
    [Route("api/news_comments")]
    [ApiController]
    [Authorize]
    public class NewsCommentsController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public NewsCommentsController(UserManager<AppUser> userManager, UnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetNewsCommentsAsync([Required] Guid newsEntryId, [FromQuery] PagingParameters parameters)
        {
            var newsEntry = await _unitOfWork.NewsEntries.GetByIdAsync(newsEntryId);
            if (newsEntry == null)
                return NotFound();

            var comments = await _unitOfWork.Comments.GetCommentDtosAsync(newsEntryId, parameters);

            return Ok(comments);
        }

        [HttpPost]
        public async Task<IActionResult> PostCommentAsync([FromBody] PostCommentRequest request)
        {
            var newsEntry = await _unitOfWork.NewsEntries.GetByIdAsync(request.NewsEntryId);
            if (newsEntry == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var comment = new Comment
            {
                Text = request.Text,
                DateTime = DateTime.UtcNow,
                NewsEntry = newsEntry,
                Author = currentUser
            };

            await _unitOfWork.Comments.AddAsync(comment);

            return Ok(new CommentDto(comment));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommentAsync(Guid id)
        {
            var comment = await _unitOfWork.Comments.GetByIdAsync(id);
            if (comment == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            if (comment.Author.Id != currentUser.Id)
                return Forbid();

            await _unitOfWork.Comments.RemoveAsync(comment);

            return Ok();
        }
    }
}
