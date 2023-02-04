using IrzUccApi.Models.PagingOptions;
using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.GetOptions
{
    public class MessagesGetParameters : SearchStringParameters
    {
        [Required]
        public int ChatId { get; set; }
        public int? LastMessageId { get; set; }
    }
}
