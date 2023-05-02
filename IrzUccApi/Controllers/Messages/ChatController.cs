using IrzUccApi.Db;
using IrzUccApi.Db.Models;
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
        private readonly UnitOfWork _unitOfWork;

        public ChatController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetChatsAsync([FromQuery] PagingParameters parameters)
        {
            var currentUser = await _unitOfWork.Users.GetByClaimsAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var chats = await _unitOfWork.Chats.GetByParticipantAsync(currentUser, parameters);

            return Ok(chats);
        }
    }
}
