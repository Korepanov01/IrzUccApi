using IrzUccApi.Models.Db;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.PagingOptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
                            recipient.Image != null ? recipient.Image.Id : null);
                    var lastMessageDto = c.LastMessage != null ? new MessageDto(
                            c.LastMessage.Id,
                            c.LastMessage.Text,
                            c.LastMessage.Image != null ? c.LastMessage.Image.Id.ToString() : null,
                            c.LastMessage.DateTime,
                            c.LastMessage.Sender.Id) : null;

                    return new ChatDto(
                        c.Id,
                        recipientDto,
                        lastMessageDto,
                        c.Messages.Where(m => m.Sender.Id != currentUser.Id && !m.IsReaded).Count());
                }));
        }
    }
}
