﻿using IrzUccApi.Db.Models;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.PagingOptions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IrzUccApi.Db.Repositories
{
    public class ChatRepository : AppRepository<Chat, AppDbContext>
    {
        public ChatRepository(AppDbContext dbContext) : base(dbContext) { }

        public IEnumerable<ChatDto> GetChatDtos(AppUser currentUser, PagingParameters parameters)
        {
            return currentUser.Chats
                .OrderByDescending(c => c.LastMessage != null ? c.LastMessage.DateTime : DateTime.MinValue)
                .Skip(parameters.PageSize * parameters.PageIndex)
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
                })
                .ToArray();
        }

        public async Task<Chat?> GetByParticipantsAsync(AppUser currentUser, AppUser recipient)
        {
            Expression<Func<Chat, bool>> filter = currentUser.Id != recipient.Id 
                ? c => c.Participants.Contains(currentUser) && c.Participants.Contains(recipient)
                : c => c.Participants.Contains(currentUser) && c.Participants.Count == 1;
        
            return await _dbContext.Chats.FirstOrDefaultAsync(filter);
        }

        public Guid? GetRecipientId(AppUser currentUser, Chat chat)
            => chat.Participants.FirstOrDefault(u => u.Id != currentUser.Id)?.Id;
    }
}
