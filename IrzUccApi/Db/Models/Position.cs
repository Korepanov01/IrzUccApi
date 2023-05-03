using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Db.Models
{
    [Table("Position")]
    [Index(nameof(Name), IsUnique = true)]
    public class Position : BaseDbModel
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<AppUser> Users { get; set; } = new HashSet<AppUser>();
    }
}