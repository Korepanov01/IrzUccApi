using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Models
{
    [Table("NewsEntry")]
    public class NewsEntry : BaseModel
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public byte[] Image { get; set; }
        public DateTime DateTime { get; } = DateTime.UtcNow;

        public virtual User Author { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<User> Likers { get; set; }
    }
}
