namespace IrzUccApi.Models.Dtos
{
    public class ChatDto
    {
        public ChatDto(
            int id, 
            UserHeaderDto recipient, 
            MessageDto? lastMessage, 
            int unreadedCount)
        {
            Id = id;
            Recipient = recipient;
            LastMessage = lastMessage;
            UnreadedCount = unreadedCount;
        }

        public int Id { get; }
        public UserHeaderDto Recipient { get; }
        public MessageDto? LastMessage { get; }
        public int UnreadedCount { get; }
    }
}
