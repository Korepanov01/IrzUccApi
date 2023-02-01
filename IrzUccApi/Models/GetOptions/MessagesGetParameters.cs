using IrzUccApi.Models.PagingOptions;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Printing;

namespace IrzUccApi.Models.GetOptions
{
    public class MessagesGetParameters : SearchStringParameters
    {
        [Required]
        public int ChatId { get; set; }
        public int? LastMessageId { get; set; }
    }
}
