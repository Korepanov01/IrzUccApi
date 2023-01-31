using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Requests.Messages
{
    public class PostMessageRequest
    {
        [Required]
        public int ChatId { get; set; }
        [MaxLength(150)]
        public string? Text { get; set; }
        public string? Image { get; set; }
    }
}
