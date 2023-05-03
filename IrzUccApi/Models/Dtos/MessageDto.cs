namespace IrzUccApi.Models.Dtos
{
    public record MessageDto
    {
        public MessageDto(
            Guid id,
            string? text,
            string? imagePath,
            DateTime dateTime,
            Guid senderId)
        {
            Id = id;
            Text = text;
            ImagePath = imagePath;
            DateTime = dateTime;
            SenderId = senderId;
        }

        public Guid Id { get; }
        public string? Text { get; }
        public string? ImagePath { get; }
        public DateTime DateTime { get; }
        public Guid SenderId { get; }
    }
}
