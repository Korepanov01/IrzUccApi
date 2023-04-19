namespace IrzUccApi.Models.Dtos
{
    public class MessageDto
    {
        public MessageDto(
            Guid id,
            string? text,
            Guid? imageId,
            DateTime dateTime,
            Guid senderId)
        {
            Id = id;
            Text = text;
            ImageId = imageId;
            DateTime = dateTime;
            SenderId = senderId;
        }

        public Guid Id { get; }
        public string? Text { get; }
        public Guid? ImageId { get; }
        public DateTime DateTime { get; }
        public Guid SenderId { get; }
    }
}
