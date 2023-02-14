using IrzUccApi.Db;
using IrzUccApi.Models.Db;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.PagingOptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace IrzUccApi.Controllers.News
{
    [Route("api/subscriptions")]
    [ApiController]
    [Authorize]
    public class SubscriptionsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public SubscriptionsController(AppDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet("user_subscribers")]
        public async Task<IActionResult> GetUserSubscribers([Required] Guid userId, [FromQuery] PagingParameters parameters)
            => await GetSubscriptionsOrSubscribers(userId, parameters, false);

        [HttpGet("my_subscribers")]
        public async Task<IActionResult> GetMySubscribers([FromQuery] PagingParameters parameters)
        {
            var myId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (myId == null)
                return Unauthorized();
            return await GetSubscriptionsOrSubscribers(new Guid(myId), parameters, false);
        }

        [HttpGet("user_subscriptions")]
        public async Task<IActionResult> GetUserSubscriptions([Required] Guid userId, [FromQuery] PagingParameters parameters)
            => await GetSubscriptionsOrSubscribers(userId, parameters, true);

        [HttpGet("my_subscriptions")]
        public async Task<IActionResult> GetMySubscriptions([FromQuery] PagingParameters parameters)
        {
            var myId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (myId == null)
                return Unauthorized();
            return await GetSubscriptionsOrSubscribers(new Guid(myId), parameters, true);
        }

        private async Task<IActionResult> GetSubscriptionsOrSubscribers(Guid userId, PagingParameters parameters, bool isSubscriptions)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return NotFound();

            var resultUsers = isSubscriptions ? user.Subscriptions : user.Subscribers;
            return Ok(resultUsers
                .OrderBy(u => u.FirstName + u.Surname + u.Patronymic + u.Email)
                .Skip(parameters.PageSize * (parameters.PageIndex - 1))
                .Take(parameters.PageSize)
                .Select(u => new UserListItemDto(
                        u.Id,
                        u.FirstName,
                        u.Surname,
                        u.Patronymic,
                        u.Email,
                        u.IsActiveAccount,
                        u.Image?.Id,
                        u.UserRoles.Select(ur => ur.Role != null ? ur.Role.Name : ""),
                        u.UserPosition.Where(up => up.End == null).Select(up => new PositionDto(up.Position.Id, up.Position.Name)))));
        }

        [HttpPost("subcribe")]
        public async Task<IActionResult> Subscribe([Required] Guid userId)
            => await SubscribeOrUnsubscribe(userId, true);

        [HttpPost("unsubscribe")]
        public async Task<IActionResult> Unsubscribe([Required] Guid userId)
            => await SubscribeOrUnsubscribe(userId, false);

        private async Task<IActionResult> SubscribeOrUnsubscribe(Guid userId, bool isSubscribe)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return NotFound();

            var myUser = await _userManager.GetUserAsync(User);
            if (myUser == null)
                return Unauthorized();

            if (user.Id == myUser.Id)
                return BadRequest();

            if (isSubscribe)
            {
                if (!user.Subscribers.Contains(myUser))
                    user.Subscribers.Add(myUser);
            }
            else
            {
                if (user.Subscribers.Contains(myUser))
                    user.Subscribers.Remove(myUser);
            }

            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
