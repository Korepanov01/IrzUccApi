using IrzUccApi.Models.Db;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.PagingOptions;
using IrzUccApi.Models.Requests.News;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Controllers.News
{
    [Route("api/news_comments")]
    [ApiController]
    [Authorize]
    public class NewsCommentsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public NewsCommentsController(AppDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetNewsComments([Required] Guid newsEntryId, [FromQuery] PagingParameters parameters)
        {
            var newsEntry = await _dbContext.NewsEntries.FirstOrDefaultAsync(n => n.Id == newsEntryId);
            if (newsEntry == null)
                return NotFound();

            return Ok(newsEntry.Comments
                .OrderBy(c => c.DateTime)
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
                        c?.Author?.Image?.Id.ToString()))));
        }

        [HttpPost]
        public async Task<IActionResult> PostComment([FromBody] PostCommentRequest request)
        {
            var newsEntry = await _dbContext.NewsEntries.FirstOrDefaultAsync(n => n.Id == request.NewsEntryId);
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

            await _dbContext.AddAsync(comment);

            await _dbContext.SaveChangesAsync();

            return Ok(new CommentDto(
                comment.Id,
                comment.Text,
                comment.DateTime,
                new UserHeaderDto(
                        comment.Author.Id,
                        comment.Author.FirstName,
                        comment.Author.Surname,
                        comment.Author.Patronymic,
                        comment?.Author?.Image?.Id.ToString())));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var comment = await _dbContext.Comments.FirstOrDefaultAsync(n => n.Id == id);
            if (comment == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            if (comment.Author.Id != currentUser.Id)
                return Forbid();

            _dbContext.Remove(comment);

            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
