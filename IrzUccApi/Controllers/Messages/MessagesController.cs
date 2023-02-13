using IrzUccApi.Enums;
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
        public async Task<IActionResult> GetMessages([FromQuery] MessagesGetParameters parameters)
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
                var normalizedSearchString = parameters.SearchString.ToUpper();
                messages = messages.Where(m => m.Text != null && m.Text.Contains(parameters.SearchString));
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
                    m.Image != null ? m.Image.Id.ToString() : null,
                    m.DateTime,
                    m.Sender.Id))
                .ToArray();

            var unreadedMessages = chat.Messages.Where(m => m.Sender.Id != currentUser.Id && !m.IsReaded);
            foreach (var unreadedMessage in unreadedMessages)
                unreadedMessage.IsReaded = true;
            await _dbContext.SaveChangesAsync();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> PostMessage([FromBody] PostMessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text) && request.Image == null)
                return BadRequest(RequestErrorMessages.MessageCantBeEmpty);

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var recipient = await _userManager.FindByIdAsync(request.UserId);
            if (recipient == null)
                return BadRequest(RequestErrorMessages.UserDoesntExistMessage);

            var chat = await _dbContext.Chats.FirstOrDefaultAsync(c => c.Participants.Contains(currentUser) && c.Participants.Contains(recipient));
            if (chat == null)
            {
                chat = new Chat
                {
                    Participants = currentUser.Id != recipient.Id
                        ? new[] { currentUser, recipient }
                        : new[] { currentUser }
                };
                await _dbContext.AddAsync(chat);
                await _dbContext.SaveChangesAsync();
            }

            Image? image = null;
            if (request.Image != null)
            {
                image = new Image
                {
                    Id = Guid.NewGuid(),
                    Name = request.Image.Name,
                    Extension = request.Image.Extension,
                    Data = request.Image.Data
                };
                await _dbContext.Images.AddAsync(image);
            }

            var message = new Message
            {
                Text = request.Text,
                Image = image,
                IsReaded = false,
                DateTime = DateTime.UtcNow,
                Sender = currentUser,
                Chat = chat
            };
            await _dbContext.AddAsync(message);
            await _dbContext.SaveChangesAsync();

            chat.LastMessage = message;
            _dbContext.Update(chat);
            await _dbContext.SaveChangesAsync();

            return Ok(new MessageDto(
                message.Id,
                message.Text,
                message.Image != null ? message.Image.Id.ToString() : null,
                message.DateTime,
                message.Sender.Id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(Guid id)
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
