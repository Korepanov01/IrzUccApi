using IrzUccApi.Enums;
using IrzUccApi.Models.Db;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.PagingOptions;
using IrzUccApi.Models.Requests.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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

            var unreadedMessages = chat.Messages.Where(m => m.Sender.Id != currentUser.Id).ToList();
            unreadedMessages.ForEach(m => m.IsReaded = true);
            _dbContext.UpdateRange(unreadedMessages);
            await _dbContext.SaveChangesAsync();

            return Ok(chat.Messages
                .Where(m => parameters.SearchString == null || m.Text != null && m.Text.Contains(parameters.SearchString))
                .OrderByDescending(m => m.DateTime)
                .Skip(parameters.PageSize * (parameters.PageIndex - 1))
                .Take(parameters.PageSize)
                .Select(m => new MessageDto(
                    m.Id,
                    m.Text,
                    m.IsReaded,
                    m.Image,
                    m.DateTime,
                    m.Sender.Id))
                .ToArray());
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

            var message = new Message
            {
                Text = request.Text,
                Image = request.Image,
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
