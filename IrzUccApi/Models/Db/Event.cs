using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Models.Db
{
    [Table("Event")]
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string? Description { get; set; }
        public Cabinet? Cabinet { get; set; }

        public virtual AppUser Creator { get; set; } = new AppUser();
        public virtual ICollection<AppUser> Listeners { get; set; } = new HashSet<AppUser>();
    }
}
