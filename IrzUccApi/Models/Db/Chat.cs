using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Models.Db
{
    [Table("Chat")]
    public class Chat
    {
        public int Id { get; set; }
        public virtual ICollection<AppUser> Participants { get; set; } = new HashSet<AppUser>();
        public virtual Message? LastMessage { get; set; }
        public virtual ICollection<Message> Messages { get; set; } = new HashSet<Message>();
    }
}
