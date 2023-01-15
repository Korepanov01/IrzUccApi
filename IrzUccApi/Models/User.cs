using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Models
{
    [Table("User")]
    public class User : BaseModel
    {
        [EmailAddress]
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly EmploymentDate { get; set; }
        public bool IsActiveAccount { get; set; }
        public DateOnly? Birthday { get; set; }
        public string? Patronymic { get; set; }
        public byte[]? Image { get; set; }
        public string? AboutMyself { get; set; }
        public string? MyDoings { get; set; }
        public string? Skills { get; set; }

        public virtual Role Role { get; set; }
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
