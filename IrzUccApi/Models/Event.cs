using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Models
{
    [Table("Event")]
    public class Event : BaseModel
    {
        public string Title { get; set; }
        public DateTime DateTime { get; set; }
        public string Periodic { get; set; }
        public string? Description { get; set; }
        public string? CabinetName { get; set; }

        public virtual User Creator { get; set; }
        public virtual ICollection<User> Listeners { get; set; }
    }
}
