using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Models.Db
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

        public virtual ICollection<Chat> Chats { get; set; } = new HashSet<Chat>();
        public virtual ICollection<AppUserRole> UserRoles { get; set; } = new HashSet<AppUserRole>();
        public virtual ICollection<AppUser> Subscriptions { get; set; } = new HashSet<AppUser>();
        public virtual ICollection<AppUser> Subscribers { get; set; } = new HashSet<AppUser>();
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public virtual ICollection<Event> Events { get; set; } = new HashSet<Event>();
        public virtual ICollection<Event> ListeningEvents { get; set; } = new HashSet<Event>();
        public virtual ICollection<NewsEntry> LikedNewsEntries { get; set; } = new HashSet<NewsEntry>();
        public virtual ICollection<NewsEntry> NewsEntries { get; set; } = new HashSet<NewsEntry>();
        public virtual ICollection<UserPosition> UserPosition { get; set; } = new HashSet<UserPosition>();
    }
}
