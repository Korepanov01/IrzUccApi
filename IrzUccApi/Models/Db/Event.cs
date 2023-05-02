using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Models.Db
{
    [Table("Event")]
    public class Event : BaseDbModel
    {
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public virtual Cabinet? Cabinet { get; set; }
        public bool IsPublic { get; set; } = false;

        public virtual AppUser Creator { get; set; } = new AppUser();
        public virtual ISet<AppUser> Listeners { get; set; } = new HashSet<AppUser>();
    }
}
