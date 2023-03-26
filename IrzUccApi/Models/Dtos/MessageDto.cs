namespace IrzUccApi.Models.Dtos
{
    public class MessageDto
    {
        public MessageDto(
            Guid id,
            string? text,
            Guid? imageId,
            DateTime dateTime,
            Guid senderId,
            bool canBeDeleted)
        {
            Id = id;
            Text = text;
            ImageId = imageId;
            DateTime = dateTime;
            SenderId = senderId;
            CanBeDeleted = canBeDeleted;
        }

        public Guid Id { get; }
        public string? Text { get; }
        public Guid? ImageId { get; }
        public DateTime DateTime { get; }
        public Guid SenderId { get; }
        public bool CanBeDeleted { get; }
    }
}
