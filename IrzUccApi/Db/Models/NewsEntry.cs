using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Db.Models
{
    [Table("NewsEntry")]
    public class NewsEntry : BaseDbModel, IEntity
    {
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(5000)]
        public string Text { get; set; } = string.Empty;
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
        public bool IsPublic { get; set; } = false;

        public virtual Image? Image { get; set; }

        public virtual AppUser Author { get; set; } = new AppUser();
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public virtual ICollection<AppUser> Likers { get; set; } = new HashSet<AppUser>();
    }
}
