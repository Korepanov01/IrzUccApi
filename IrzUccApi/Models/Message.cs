using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Models
{
    [Table("Message")]
    public class Message : BaseModel
    {
        public string Text { get; set; }
        public bool IsReaded { get; set; }
        public byte[] Image { get; set; }
        public DateTime DateTime { get; } = DateTime.UtcNow;

        public virtual User Sender { get; set; }
    }
}
