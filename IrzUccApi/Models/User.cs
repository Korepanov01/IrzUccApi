using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Models
{
    [Table("User")]
    public class User : BaseModel
    {
        public virtual Position Position { get; set; }
        public virtual ICollection<User> Subscriptions { get; set; }
        public virtual ICollection<User> Subscribers { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Event> MyEvents { get; set; }
        public virtual ICollection<Event> ListeningEvents { get; set; }
        public virtual ICollection<NewsEntry> LikedNewsEntries { get; set; }
        public virtual ICollection<NewsEntry> MyNewsEntries { get; set; }
    }
}
