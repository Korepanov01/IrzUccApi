using IrzUccApi.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace IrzUccApi.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                await Clients.Caller.SendAsync(ChatHubMethodsNames.Unauthorized);
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, userId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                await Clients.Caller.SendAsync(ChatHubMethodsNames.Unauthorized);
                return;
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);

            await base.OnDisconnectedAsync(exception);
        }

        private string? GetUserId()
        {
            if (Context?.User?.Identity == null || !Context.User.Identity.IsAuthenticated)
            {
                return null;
            }

            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return userId;
        }
    }
}