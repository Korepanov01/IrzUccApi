using IrzUccApi.Db.Models;

namespace IrzUccApi.Models.Dtos
{
    public record NewsEntryDto
    {
        public NewsEntryDto(NewsEntry newsEntry, AppUser currentUser) : this(
            newsEntry.Id,
            newsEntry.Title,
            newsEntry.Text,
            newsEntry.Image?.Id,
            newsEntry.DateTime,
            currentUser != null && currentUser.LikedNewsEntries.Contains(newsEntry),
            newsEntry.Likers.Count,
            new UserHeaderDto(
                newsEntry.Author.Id,
                newsEntry.Author.FirstName,
                newsEntry.Author.Surname,
                newsEntry.Author.Patronymic,
                newsEntry.Author.Image?.Id),
            newsEntry.IsPublic,
            newsEntry.Comments.Count,
            false)
        { }


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
            int commentCount,
            bool isClipped)
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
            IsClipped = isClipped;
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
        public bool IsClipped { get; }
    }
}
