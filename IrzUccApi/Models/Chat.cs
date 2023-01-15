using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Models
{
    [Table("Chat")]
    public class Chat : BaseModel
    {
        public virtual User FirstUser { get; }
        public virtual User SecondUser { get; }
        public virtual Message? LastMessage { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}
