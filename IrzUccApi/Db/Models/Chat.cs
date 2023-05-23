using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Db.Models
{
    [Table("Chat")]
    public class Chat : BaseDbModel, IEntity
    {
        public virtual ISet<AppUser> Participants { get; set; } = new HashSet<AppUser>();
        [ForeignKey("LastMessageId")]
        public virtual Message? LastMessage { get; set; }
        [ForeignKey("ChatId")]
        public virtual ISet<Message> Messages { get; set; } = new HashSet<Message>();
    }
}
