namespace IrzUccApi.Models.Dtos
{
    public record CommentDto
    {
        public CommentDto(Guid id, string text, DateTime dateTime, UserHeaderDto user)
        {
            Id = id;
            Text = text;
            DateTime = dateTime;
            User = user;
        }

        public Guid Id { get; }
        public string Text { get; }
        public DateTime DateTime { get; }
        public UserHeaderDto User { get; }

    }
}
