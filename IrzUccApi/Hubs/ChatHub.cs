﻿using IrzUccApi.Db;
using IrzUccApi.Enums;
using IrzUccApi.ErrorDescribers;
using IrzUccApi.Models.Db;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.Requests.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRSwaggerGen.Attributes;
using System.Security.Claims;

namespace IrzUccApi.Hubs
{
    [SignalRHub]
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public ChatHub(
            AppDbContext dbContext, 
            UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
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

        public async Task SendMessage(PostMessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text) && request.Image == null)
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

            Image? image = null;
            if (request.Image != null)
            {
                image = new Image
                {
                    Name = request.Image.Name,
                    Extension = request.Image.Extension,
                    Data = request.Image.Data,
                    Source = ImageSources.Message,
                    SourceId = newMessageId
                };
                await _dbContext.Images.AddAsync(image);
            }

            var message = new Message
            {
                Id = newMessageId,
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

            var messageDto = new MessageDto(
                    message.Id,
                    message.Text,
                    message.Image?.Id,
                    message.DateTime,
                    message.Sender.Id);

            await Clients
                .Group(currentUser.Id.ToString())
                .SendAsync(ChatHubMethodsNames.MessageReceived, messageDto);
            await Clients
                .Group(request.UserId)
                .SendAsync(ChatHubMethodsNames.MessageReceived, messageDto);
        }

        public async Task DeleteMessage(DeleteMessageRequest request)
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