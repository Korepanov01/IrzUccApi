using IrzUccApi.Db.Models;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.GetOptions;
using IrzUccApi.Models.PagingOptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IrzUccApi.Db.Repositories
{
    public class ChatRepository : AppRepository<Chat, AppDbContext>
    {
        public ChatRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<ChatDto>> GetByParticipantAsync(AppUser user, PagingParameters parameters)
        {
            var chats = (await _dbContext.Chats
                .Where(c => c.Participants.Contains(user))
                .OrderByDescending(c => c.LastMessage != null ? c.LastMessage.DateTime : DateTime.MinValue)
                .Skip(parameters.PageSize * (parameters.PageIndex - 1))
                .Take(parameters.PageSize)
                .ToArrayAsync())
                .Select(c =>
                {
                    var recipient = c.Participants.FirstOrDefault(u => u.Id != user.Id) ?? user;
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
                        c.Messages.Where(m => m.Sender.Id != user.Id && !m.IsReaded).Count());
                })
                .ToArray();

            return chats;
        }
    }
}
