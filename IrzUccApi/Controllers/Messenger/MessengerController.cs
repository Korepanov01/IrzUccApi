﻿using IrzUccApi.Db;
using IrzUccApi.Db.Models;
using IrzUccApi.Db.Repositories;
using IrzUccApi.Enums;
using IrzUccApi.ErrorDescribers;
using IrzUccApi.Hubs;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.GetOptions;
using IrzUccApi.Models.PagingOptions;
using IrzUccApi.Models.Requests.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace IrzUccApi.Controllers.Messages
{
    [Route("api/messenger")]
    [ApiController]
    [Authorize]
    public class MessengerController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHubContext<ChatHub> _chatHub;

        public MessengerController(
            UnitOfWork unitOfWork, 
            UserManager<AppUser> userManager,
            IHubContext<ChatHub> chatHub)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _chatHub = chatHub;
        }

        [HttpGet("chats")]
        public async Task<IActionResult> GetChatsAsync([FromQuery] PagingParameters parameters)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var chats = _unitOfWork.Chats.GetChatDtos(currentUser, parameters);

            return Ok(chats);
        }

        [HttpGet("chats/by_participant")]
        public async Task<IActionResult> GetChatIdByRecipientIdAsync([FromQuery] Guid participantId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var participant = await _userManager.FindByIdAsync(participantId.ToString());
            if (participant == null)
                return NotFound();

            var chat = await _unitOfWork.Chats
                .GetOrCreateByParticipantsAsync(currentUser, participant);

            return Ok(chat.Id);
        }

        [HttpGet("messages")]
        public async Task<IActionResult> GetMessagesAsync([FromQuery] MessagesGetParameters parameters)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var chat = await _unitOfWork.Chats.GetByIdAsync(parameters.ChatId);
            if (chat == null)
                return NotFound();

            if (!chat.Participants.Contains(currentUser))
                return Forbid();

            var messages = await _unitOfWork.Messages.GetMessagesDtosAsync(parameters);

            await _unitOfWork.Messages.MakeMessagesReadedAsync(chat.Id, currentUser.Id);

            return Ok(messages);
        }

        [HttpPost("messages")]
        public async Task<IActionResult> SendMessageAsync([FromBody] SendMessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text) && request.Image == null)
                return BadRequest(new[] { RequestErrorDescriber.MessageCantBeEmpty });

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var recipient = await _userManager.FindByIdAsync(request.UserId);
            if (recipient == null)
                return BadRequest(new[] { RequestErrorDescriber.UserDoesntExist });

            var chat = await _unitOfWork.Chats.GetOrCreateByParticipantsAsync(currentUser, recipient);

            var newMessageId = Guid.NewGuid();

            Image? image = null;
            if (request.Image != null)
            {
                try
                {
                    image = await _unitOfWork.Images.AddAsync(request.Image);
                }
                catch (FileTooBigException)
                {
                    return BadRequest(RequestErrorDescriber.FileTooBig);
                }
                catch (ForbiddenFileExtensionException)
                {
                    return BadRequest(RequestErrorDescriber.ForbiddenExtention);
                }
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
            await _unitOfWork.Messages.AddAsync(message);

            chat.LastMessage = message;
            await _unitOfWork.Chats.UpdateAsync(chat);

            var messageDto = new MessageDto(message);
            await _chatHub.Clients
                .Group(currentUser.Id.ToString())
                .SendAsync(ChatHubMethodsNames.MessageReceived, messageDto);
            if (currentUser.Id.ToString() != request.UserId)
                await _chatHub.Clients
                    .Group(request.UserId)
                    .SendAsync(ChatHubMethodsNames.MessageReceived, messageDto);

            return Ok();
        }

        [HttpDelete("messages/{id}")]
        public async Task<IActionResult> DeleteMessageAsync(Guid id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var message = await _unitOfWork.Messages.GetByIdAsync(id);
            if (message == null)
                return NotFound();

            if (message.Sender.Id != currentUser.Id)
                return Forbid();
            
            var chat = message.Chat;
            if (chat.LastMessage?.Id == message.Id)
            {
                chat.LastMessage = _unitOfWork.Messages.GetPenultimateMessage(chat);
                await _unitOfWork.Chats.UpdateAsync(chat);
            }

            await _unitOfWork.Messages.RemoveAsync(message);

            var recipientId = _unitOfWork.Chats.GetRecipientId(currentUser, chat);
            await _chatHub.Clients
                .Group(currentUser.Id.ToString())
                .SendAsync(ChatHubMethodsNames.MessageDeleted, id);
            if (recipientId != null)
                await _chatHub.Clients
                    .Group(recipientId.ToString()!)
                    .SendAsync(ChatHubMethodsNames.MessageDeleted, id);

            return Ok();
        }
    }
}
