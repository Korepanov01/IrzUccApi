using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Models.Db
{
    [Table("Message")]
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsReaded { get; set; }
        public byte[] Image { get; set; } = Array.Empty<byte>();
        public DateTime DateTime { get; set; } = DateTime.UtcNow;

        public virtual AppUser Sender { get; set; } = new AppUser();
        public virtual Chat Chat { get; set; } = new Chat();
    }
}
