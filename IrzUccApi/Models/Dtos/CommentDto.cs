using IrzUccApi.Db.Models;

namespace IrzUccApi.Models.Dtos
{
    public record CommentDto
    {
        public CommentDto(Comment comment) : this(
            comment.Id,
            comment.Text,
            comment.DateTime,
            new UserHeaderDto(
                comment.Author.Id,
                comment.Author.FirstName,
                comment.Author.Surname,
                comment.Author.Patronymic,             
                comment?.Author?.Image?.Id)) { }


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
