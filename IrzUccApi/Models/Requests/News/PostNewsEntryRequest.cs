using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Requests.News
{
    public class PostNewsEntryRequest
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        [Required(AllowEmptyStrings = false)]
        [MaxLength(5000)]
        public string Text { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
        public bool IsPublic { get; set; }
    }
}
