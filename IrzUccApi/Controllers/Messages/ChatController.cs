using IrzUccApi.Db;
using IrzUccApi.Models.Db;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.PagingOptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrzUccApi.Controllers.Messages
{
    [Route("api/chats")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public ChatController(AppDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetChatsAsync([FromQuery] PagingParameters parameters)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            return Ok(currentUser.Chats
                .OrderByDescending(c => c.LastMessage != null ? c.LastMessage.DateTime : DateTime.MinValue)
                .Skip(parameters.PageSize * (parameters.PageIndex - 1))
                .Take(parameters.PageSize)
                .Select(c =>
                {
                    var recipient = c.Participants.FirstOrDefault(u => u.Id != currentUser.Id) ?? currentUser;
                    var recipientDto = new UserHeaderDto(
                            recipient.Id,
                            recipient.FirstName,
                            recipient.Surname,
                            recipient.Patronymic,
                            recipient.Image?.Id);
                    var lastMessageDto = c.LastMessage != null ? new MessageDto(
                            c.LastMessage.Id,
                            c.LastMessage.Text,
                            c.LastMessage.Image?.Id,
                            c.LastMessage.DateTime,
                            c.LastMessage.Sender.Id) : null;

                    return new ChatDto(
                        c.Id,
                        recipientDto,
                        lastMessageDto,
                        c.Messages.Where(m => m.Sender.Id != currentUser.Id && !m.IsReaded).Count());
                }));
        }

        [HttpGet("chat_by_participant")]
        public async Task<IActionResult> GetChatIdByRecipientIdAsync([FromQuery] Guid participantId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var participant = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == participantId);
            if (participant == null) 
                return NotFound();

            var chatId = _dbContext.Chats
                .FirstOrDefault(c => c.Participants.Contains(currentUser) && c.Participants.Contains(participant))?.Id;
            if (chatId == null)
                return NotFound();

            return Ok(chatId);
        }
    }
}
