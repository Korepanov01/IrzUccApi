using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace IrzUccApi.Models
{
    [Table("Position")]
    [Index(nameof(Name), IsUnique = true)]
    public class Position
    {
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<AppUser> Users { get; set; } = new HashSet<AppUser>();
    }
}