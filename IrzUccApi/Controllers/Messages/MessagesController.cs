using IrzUccApi.Db;
using IrzUccApi.Enums;
using IrzUccApi.ErrorDescribers;
using IrzUccApi.Models.Db;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.GetOptions;
using IrzUccApi.Models.Requests.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> GetMessagesAsync([FromQuery] MessagesGetParameters parameters)
        {
            var chat = await _dbContext.Chats.FirstOrDefaultAsync(c => c.Id == parameters.ChatId);
            if (chat == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            if (!chat.Participants.Contains(currentUser))
                return Forbid();

            var messages = chat.Messages.AsQueryable();

            if (parameters.SearchString != null && parameters.LastMessageId == null)
            {
                var normalizedSearchWords = parameters.SearchString
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(sw => sw.ToUpper());
                foreach (var word in normalizedSearchWords)
                    messages = messages.Where(m => m.Text != null && m.Text.ToUpper().Contains(word));
            }

            messages = parameters.LastMessageId == null
                ? messages
                    .OrderByDescending(m => m.DateTime)
                    .Skip(parameters.PageSize * (parameters.PageIndex - 1))
                : messages
                    .OrderBy(m => m.DateTime)
                    .SkipWhile(m => m.Id != parameters.LastMessageId)
                    .Skip(1);

            var result = messages
                .Take(parameters.PageSize)
                .Select(m => new MessageDto(
                    m.Id,
                    m.Text,
                    m.ImagePath ?? null,
                    m.DateTime,
                    m.Sender.Id))
                .ToArray();

            var unreadedMessages = chat.Messages.Where(m => m.Sender.Id != currentUser.Id && !m.IsReaded);
            foreach (var unreadedMessage in unreadedMessages)
                unreadedMessage.IsReaded = true;
            await _dbContext.SaveChangesAsync();

            return Ok(result);
        }
    }
}
