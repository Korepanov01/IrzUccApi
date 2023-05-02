using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Models.Db
{
    [Table("Chat")]
    public class Chat : BaseDbModel, IEntity
    {
        public virtual ICollection<AppUser> Participants { get; set; } = new HashSet<AppUser>();
        [ForeignKey("LastMessageId")]
        public virtual Message? LastMessage { get; set; }
        [ForeignKey("ChatId")]
        public virtual ICollection<Message> Messages { get; set; } = new HashSet<Message>();
    }
}
