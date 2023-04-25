namespace IrzUccApi.Models.Dtos
{
    public record ChatDto
    {
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
