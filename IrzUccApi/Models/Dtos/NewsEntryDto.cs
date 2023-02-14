namespace IrzUccApi.Models.Dtos
{
    public class NewsEntryDto
    {
        public NewsEntryDto(
            Guid id,
            string title,
            string text,
            Guid? imageId,
            DateTime dateTime,
            bool isLiked,
            int likesCount,
            UserHeaderDto author,
            bool isPublic,
            int commentCount)
        {
            Id = id;
            Title = title;
            Text = text;
            ImageId = imageId;
            DateTime = dateTime;
            IsLiked = isLiked;
            LikesCount = likesCount;
            Author = author;
            IsPublic = isPublic;
            CommentCount = commentCount;
        }

        public Guid Id { get; }
        public string Title { get; }
        public string Text { get; }
        public Guid? ImageId { get; }
        public DateTime DateTime { get; }
        public bool IsLiked { get; }
        public int LikesCount { get; }
        public UserHeaderDto Author { get; }
        public bool IsPublic { get; }
        public int CommentCount { get; }
    }
}
