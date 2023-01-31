using IrzUccApi.Enums;
using IrzUccApi.Models.Db;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.GetOptions;
using IrzUccApi.Models.PagingOptions;
using IrzUccApi.Models.Requests.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace IrzUccApi.Controllers.Messages
{
    [Route("api/messages")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public MessagesController(AppDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages([Required] int chatId, [FromQuery] SearchStringParameters parameters)
        {
            var chat = await _dbContext.Chats.FirstOrDefaultAsync(c => c.Id == chatId);
            if (chat == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            if (!chat.Participants.Contains(currentUser))
                return Forbid();

            var messages = chat.Messages.AsQueryable();

            if (parameters.SearchString != null)
                messages = messages.Where(m => m.Text.Contains(parameters.SearchString));

            messages = messages.
                OrderBy(m => m.DateTime)
                .Skip(parameters.PageSize * (parameters.PageIndex - 1))
                .Take(parameters.PageSize);

            await messages.ForEachAsync(m => m.IsReaded = true);
            _dbContext.UpdateRange(messages);
            await _dbContext.SaveChangesAsync();

            return Ok(await messages
                .Select(m => new MessageDto(
                    m.Id,
                    m.Text,
                    m.IsReaded,
                    m.Image,
                    m.DateTime,
                    m.Sender.Id))
                .ToArrayAsync());
        }

        [HttpPost]
        public async Task<IActionResult> PostMessage([FromBody] PostMessageRequest request)
        {
            var chat = await _dbContext.Chats.FirstOrDefaultAsync(c => c.Id == request.ChatId);
            if (chat == null)
                return BadRequest(RequestErrorMessages.ChatDoesntExist);

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(request.Text) && request.Image == null)
                return BadRequest(RequestErrorMessages.MessageCantBeEmpty);

            var message = new Message
            {
                Text = request.Text,
                Image = request.Image,
                IsReaded = false,
                DateTime = DateTime.UtcNow,
                Sender = currentUser,
            };

            chat.Messages.Add(message);
            await _dbContext.SaveChangesAsync();

            return Ok(new MessageDto(
                message.Id,
                message.Text,
                message.IsReaded,
                message.Image,
                message.DateTime,
                message.Sender.Id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var message = await _dbContext.Messages.FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            if (message.Sender.Id != currentUser.Id)
                return Forbid();

            _dbContext.Remove(message);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
