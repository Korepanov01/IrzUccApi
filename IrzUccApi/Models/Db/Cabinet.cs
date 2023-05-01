using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Db
{
    [Index(nameof(Name), IsUnique = true)]
    public class Cabinet : BaseDbModel, IEntity
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<Event> Events { get; set; } = new HashSet<Event>();
    }
}
