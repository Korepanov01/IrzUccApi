using IrzUccApi.Db;
using IrzUccApi.Db.Models;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.PagingOptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace IrzUccApi.Controllers.News
{
    [Route("api/subscriptions")]
    [ApiController]
    [Authorize]
    public class SubscriptionsController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public SubscriptionsController(UserManager<AppUser> userManager, UnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("user_subscribers")]
        public async Task<IActionResult> GetUserSubscribersAsync([Required] Guid userId, [FromQuery] PagingParameters parameters)
            => await GetSubscriptionsOrSubscribersAsync(userId, parameters, false);

        [HttpGet("my_subscribers")]
        public async Task<IActionResult> GetMySubscribersAsync([FromQuery] PagingParameters parameters)
        {
            var myId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (myId == null)
                return Unauthorized();
            return await GetSubscriptionsOrSubscribersAsync(new Guid(myId), parameters, false);
        }

        [HttpGet("user_subscriptions")]
        public async Task<IActionResult> GetUserSubscriptionsAsync([Required] Guid userId, [FromQuery] PagingParameters parameters)
            => await GetSubscriptionsOrSubscribersAsync(userId, parameters, true);

        [HttpGet("my_subscriptions")]
        public async Task<IActionResult> GetMySubscriptionsAsync([FromQuery] PagingParameters parameters)
        {
            var myId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (myId == null)
                return Unauthorized();
            return await GetSubscriptionsOrSubscribersAsync(new Guid(myId), parameters, true);
        }

        private async Task<IActionResult> GetSubscriptionsOrSubscribersAsync(Guid userId, PagingParameters parameters, bool isSubscriptions)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return NotFound();

            var result = _unitOfWork.Users.GetSubscriptionsOrSubscribers(user, parameters, isSubscriptions);

            return Ok(result);
        }

        [HttpPost("subcribe")]
        public async Task<IActionResult> SubscribeAsync([Required] Guid userId)
            => await SubscribeOrUnsubscribeAsync(userId, true);

        [HttpPost("unsubscribe")]
        public async Task<IActionResult> UnsubscribeAsync([Required] Guid userId)
            => await SubscribeOrUnsubscribeAsync(userId, false);

        private async Task<IActionResult> SubscribeOrUnsubscribeAsync(Guid userId, bool isSubscribe)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return NotFound();

            if (user.Id == currentUser.Id)
                return BadRequest();

            if (isSubscribe)
            {
                if (!user.Subscribers.Contains(currentUser))
                    user.Subscribers.Add(currentUser);
            }
            else
            {
                if (user.Subscribers.Contains(currentUser))
                    user.Subscribers.Remove(currentUser);
            }

            await _unitOfWork.Users.UpdateAsync(user);

            return Ok();
        }
    }
}
