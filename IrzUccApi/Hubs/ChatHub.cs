using IrzUccApi.Db;
using IrzUccApi.Enums;
using IrzUccApi.ErrorDescribers;
using IrzUccApi.Models.Db;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.Requests.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRSwaggerGen.Attributes;
using System.Security.Claims;

namespace IrzUccApi.Hubs
{
    [SignalRHub("/hubs/chat")]
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ChatHub(
            AppDbContext dbContext,
            UserManager<AppUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
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

        [SignalRHidden]
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

        [SignalRHidden]
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

        [SignalRMethod]
        public async Task SendMessageAsync([SignalRParam] PostMessageRequest request, [FromForm] IFormFile? image)
        {
            if (string.IsNullOrWhiteSpace(request.Text) && image == null)
            {
                await Clients.Caller.SendAsync(ChatHubMethodsNames.BadRequest, new[] { RequestErrorDescriber.MessageCantBeEmpty });
                return;
            }

            var currentUser = await _userManager.GetUserAsync(Context.User);
            if (currentUser == null)
            {
                await Clients.Caller.SendAsync(ChatHubMethodsNames.Unauthorized);
                return;
            }

            var recipient = await _userManager.FindByIdAsync(request.UserId);
            if (recipient == null)
            {
                await Clients.Caller.SendAsync(ChatHubMethodsNames.BadRequest, new[] { RequestErrorDescriber.UserDoesntExist });
                return;
            }

            var chat = await _dbContext.Chats
                .FirstOrDefaultAsync(c =>
                    c.Participants.Contains(currentUser) && c.Participants.Contains(recipient));
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

            var newMessageId = Guid.NewGuid();

            var message = new Message
            {
                Id = newMessageId,
                Text = request.Text,
                IsReaded = false,
                DateTime = DateTime.UtcNow,
                Sender = currentUser,
                Chat = chat
            };
            if (image != null)
            {
                var path = "/Images/" + Guid.NewGuid().ToString();

                using (var fileStream = new FileStream(_webHostEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                message.ImagePath = path;
            }

            await _dbContext.AddAsync(message);
            await _dbContext.SaveChangesAsync();

            chat.LastMessage = message;
            _dbContext.Update(chat);
            await _dbContext.SaveChangesAsync();

            var messageDto = new MessageDto(
                    message.Id,
                    message.Text,
                    message.ImagePath,
                    message.DateTime,
                    message.Sender.Id);

            await Clients
                .Group(currentUser.Id.ToString())
                .SendAsync(ChatHubMethodsNames.MessageReceived, messageDto);
            await Clients
                .Group(request.UserId)
                .SendAsync(ChatHubMethodsNames.MessageReceived, messageDto);
        }

        [SignalRMethod]
        public async Task DeleteMessageAsync([SignalRParam] DeleteMessageRequest request)
        {
            var currentUser = await _userManager.GetUserAsync(Context.User);
            if (currentUser == null)
            {
                await Clients.Caller.SendAsync(ChatHubMethodsNames.Unauthorized);
                return;
            }

            var message = await _dbContext.Messages.FirstOrDefaultAsync(m => m.Id == request.MessageId);
            if (message == null)
            {
                await Clients.Caller.SendAsync(ChatHubMethodsNames.NotFound);
                return;
            }

            if (message.Sender.Id != currentUser.Id)
            {
                await Clients.Caller.SendAsync(ChatHubMethodsNames.Forbidden);
                return;
            }

            var recipient = message.Chat.Participants.FirstOrDefault(u => u.Id != currentUser.Id);

            if (message.ImagePath != null)
            {
                var file = new FileInfo(_webHostEnvironment.WebRootPath + message.ImagePath);
                if (file.Exists)
                    file.Delete();
            }
            _dbContext.Remove(message);
            await _dbContext.SaveChangesAsync();

            await Clients
                .Group(currentUser.Id.ToString())
                .SendAsync(ChatHubMethodsNames.MessageDeleted, request.MessageId);

            if (recipient != null)
                await Clients
                    .Group(recipient.Id.ToString())
                    .SendAsync(ChatHubMethodsNames.MessageReceived, request.MessageId);
        }
    }
}