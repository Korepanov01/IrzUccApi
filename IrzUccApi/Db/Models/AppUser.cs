using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Db.Models
{
    [Table("User")]
    public class AppUser : IdentityUser<Guid>, IEntity
    {
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        [MaxLength(50)]
        public string Surname { get; set; } = string.Empty;
        public bool IsActiveAccount { get; set; } = true;
        [MaxLength(50)]
        public string? Patronymic { get; set; }
        public DateTime Birthday { get; set; }
        public string? AboutMyself { get; set; }
        public string? MyDoings { get; set; }
        public string? Skills { get; set; }
        public string? RefreshToken { get; set; }

        public virtual Image? Image { get; set; }

        public virtual ISet<Chat> Chats { get; set; } = new HashSet<Chat>();
        public virtual ISet<AppUserRole> UserRoles { get; set; } = new HashSet<AppUserRole>();
        public virtual ISet<AppUser> Subscriptions { get; set; } = new HashSet<AppUser>();
        public virtual ISet<AppUser> Subscribers { get; set; } = new HashSet<AppUser>();
        public virtual ISet<Comment> Comments { get; set; } = new HashSet<Comment>();
        public virtual ISet<Event> Events { get; set; } = new HashSet<Event>();
        public virtual ISet<Event> ListeningEvents { get; set; } = new HashSet<Event>();
        public virtual ISet<NewsEntry> LikedNewsEntries { get; set; } = new HashSet<NewsEntry>();
        public virtual ISet<NewsEntry> NewsEntries { get; set; } = new HashSet<NewsEntry>();
        public virtual ISet<UserPosition> UserPosition { get; set; } = new HashSet<UserPosition>();
    }
}
