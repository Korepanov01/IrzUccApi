using IrzUccApi.Db.Models;

namespace IrzUccApi.Models.Dtos
{
    public record MessageDto
    {
        public MessageDto(Message message) : this(
            message.Id,
            message.Text,
            message.Image?.Id,
            message.DateTime,
            message.Sender.Id) { }

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
