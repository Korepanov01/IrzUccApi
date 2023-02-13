using IrzUccApi.Models.PagingOptions;
using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.GetOptions
{
    public class MessagesGetParameters : SearchStringParameters
    {
        [Required]
        public Guid ChatId { get; set; }
        public Guid? LastMessageId { get; set; }
    }
}
