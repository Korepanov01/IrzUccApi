using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Models
{
    [Table("Chat")]
    public class Chat
    {
        public int Id { get; set; }
        public virtual AppUser FirstUser { get; set; } = new AppUser();
        public virtual AppUser SecondUser { get; set; } = new AppUser();
        public virtual Message? LastMessage { get; set; }
        public virtual ICollection<Message> Messages { get; set; } = new HashSet<Message>();
    }
}
