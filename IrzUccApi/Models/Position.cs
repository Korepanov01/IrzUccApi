using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Models
{
    [Table("Position")]
    public class Position
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<AppUser> Users { get; set; } = new HashSet<AppUser>();
    }
}