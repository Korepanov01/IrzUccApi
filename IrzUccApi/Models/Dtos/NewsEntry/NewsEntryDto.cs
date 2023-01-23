namespace IrzUccApi.Models.Dtos.NewsEntry
{
    public class NewsEntryDto
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? Text { get; set; }
        public string? Image { get; set; }
        public DateTime? DateTime { get; set; }
        public bool? IsLiked { get; set; }
        public int? LikesCount { get; set; }
        public string? AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public bool? IsPublic { get; set; }
    }
}
