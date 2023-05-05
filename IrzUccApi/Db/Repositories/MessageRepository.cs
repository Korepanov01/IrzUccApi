using IrzUccApi.Db.Models;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.GetOptions;
using Microsoft.EntityFrameworkCore;
using System;

namespace IrzUccApi.Db.Repositories
{
    public class MessageRepository : AppRepository<Message, AppDbContext>
    {
        public MessageRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<MessageDto>> GetMessagesDtosAsync(MessagesGetParameters parameters)
        {
            var messages = _dbContext.Messages
                .Where(m => m.Chat.Id == parameters.ChatId);

            if (parameters.SearchString != null && parameters.LastMessageId == null)
            {
                var normalizedSearchWords = parameters.SearchString
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(sw => sw.ToUpper());
                foreach (var word in normalizedSearchWords)
                    messages = messages.Where(m => m.Text != null && m.Text.ToUpper().Contains(word));
            }

            messages = parameters.LastMessageId == null
                ? messages
                    .OrderByDescending(m => m.DateTime)
                    .Skip(parameters.PageSize * (parameters.PageIndex - 1))
                : messages
                    .OrderBy(m => m.DateTime)
                    .SkipWhile(m => m.Id != parameters.LastMessageId)
                    .Skip(1);

            return await messages
                .Take(parameters.PageSize)
                .Select(m => new MessageDto(
                    m.Id,
                    m.Text,
                    m.Image != null ? m.Image.Id : null,
                    m.DateTime,
                    m.Sender.Id))
                .ToArrayAsync();
        }

        public async Task MakeMessagesReadedAsync(Guid chatId, Guid currentUserId)
        {
            await _dbContext.Messages
                .Where(m => m.Chat.Id == chatId)
                .Where(m => m.Sender.Id != currentUserId && !m.IsReaded)
                .ForEachAsync(m => m.IsReaded = true);
            await _dbContext.SaveChangesAsync();
        }

        public Guid? GetRecipientIdByMessage(AppUser currentUser, Message message)
            => message.Chat.Participants.FirstOrDefault(u => u.Id != currentUser.Id)?.Id;
    }
}
