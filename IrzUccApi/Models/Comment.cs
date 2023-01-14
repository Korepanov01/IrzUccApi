using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Models
{
    [Table("Comment")]
    public class Comment : BaseModel
    {
        public string Text { get; set; }
        public DateTime DateTime { get; } = DateTime.UtcNow;

        public virtual NewsEntry NewsEntry { get; set; }
        public virtual User User { get; set; }
    }
}
