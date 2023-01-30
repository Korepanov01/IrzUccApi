using IrzUccApi.Models.PagingOptions;

namespace IrzUccApi.Models.GetOptions
{
    public class NewsSearchParameters : SearchStringParameters
    {
        public string? AuthorId { get; set; }
        public bool PublicOnly { get; set; } = false;
        public bool LikedOnly { get; set; } = false;
    }
}
