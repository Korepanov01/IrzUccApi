using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Requests.News
{
    public class PostCommentRequest
    {
        [Required]
        public Guid NewsEntryId { get; set; }
        [Required(AllowEmptyStrings = false)]
        [MaxLength(250)]
        public string Text { get; set; } = string.Empty;
    }
}
