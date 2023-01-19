using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Models
{
    [Table("NewsEntry")]
    public class NewsEntry
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public byte[] Image { get; set; } = Array.Empty<byte>();
        public DateTime DateTime { get; set; } = DateTime.UtcNow;

        public virtual AppUser Author { get; set; } = new AppUser();
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public virtual ICollection<AppUser> Likers { get; set; } = new HashSet<AppUser>();
    }
}
