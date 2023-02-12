using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Models.Db
{
    [Table("Message")]
    public class Message : BaseDbModel
    {
        [MaxLength(150)]
        public string? Text { get; set; } = string.Empty;
        public bool IsReaded { get; set; }
        public DateTime DateTime { get; set; } = DateTime.UtcNow;

        public virtual Image? Image { get; set; }

        public virtual AppUser Sender { get; set; } = new AppUser();
        public virtual Chat Chat { get; set; } = new Chat();
    }
}
