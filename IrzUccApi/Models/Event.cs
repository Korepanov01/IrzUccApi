using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Models
{
    [Table("Event")]
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public string Periodic { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CabinetName { get; set; }

        public virtual AppUser Creator { get; set; } = new AppUser();
        public virtual ICollection<AppUser> Listeners { get; set; } = new HashSet<AppUser>();
    }
}
