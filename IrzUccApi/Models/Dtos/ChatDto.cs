using IrzUccApi.Db.Models;

namespace IrzUccApi.Models.Dtos
{
    public record ChatDto
    {
        public ChatDto(Chat chat, AppUser currentUser)
        {
            var recipient = chat.Participants.FirstOrDefault(u => u.Id != currentUser.Id) ?? currentUser;
            var recipientDto = new UserHeaderDto(
                    recipient.Id,
                    recipient.FirstName,
                    recipient.Surname,
                    recipient.Patronymic,
                    recipient.Image?.Id);
            var lastMessageDto = chat.LastMessage != null ? new MessageDto(
                    chat.LastMessage.Id,
                    chat.LastMessage.Text,
                    chat.LastMessage.Image?.Id,
                    chat.LastMessage.DateTime,
                    chat.LastMessage.Sender.Id) : null;
            Id = chat.Id;
            Recipient = recipientDto;
            LastMessage = lastMessageDto;
            UnreadedCount = chat.Messages.Where(m => m.Sender.Id != currentUser.Id && !m.IsReaded).Count();
        }

        public ChatDto(
            Guid id,
            UserHeaderDto recipient,
            MessageDto? lastMessage,
            int unreadedCount)
        {
            Id = id;
            Recipient = recipient;
            LastMessage = lastMessage;
            UnreadedCount = unreadedCount;
        }

        public Guid Id { get; }
        public UserHeaderDto Recipient { get; }
        public MessageDto? LastMessage { get; }
        public int UnreadedCount { get; }
    }
}
