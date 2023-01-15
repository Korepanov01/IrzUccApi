using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Models
{
    [Table("Position")]
    public class Position : BaseModel
    {
        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}