using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Requests.Messages
{
    public class PostMessageRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string UserId { get; set; } = string.Empty;
        [MaxLength(150)]
        public string? Text { get; set; }
        public string? Image { get; set; }
    }
}
