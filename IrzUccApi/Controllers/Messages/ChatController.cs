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
        public async Task<IActionResult> GetChats([FromQuery] PagingParameters parameters) 
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
                            recipient.Image);
                    var lastMessageDto = c.LastMessage != null ? new MessageDto(
                            c.LastMessage.Id,
                            c.LastMessage.Text,
                            c.LastMessage.IsReaded,
                            c.LastMessage.Image,
                            c.LastMessage.DateTime,
                            c.LastMessage.Sender.Id) : null;

                    return new ChatDto(
                        c.Id,
                        recipientDto,
                        lastMessageDto,
                        c.Messages.Where(m => !m.IsReaded).Count());
                }));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChat(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var chat = await _dbContext.Chats.FirstOrDefaultAsync(c => c.Id == id);
            if (chat == null) 
                return NotFound();

            if (!chat.Participants.Contains(currentUser))
                return Forbid();
            
            _dbContext.Chats.Remove(chat);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
