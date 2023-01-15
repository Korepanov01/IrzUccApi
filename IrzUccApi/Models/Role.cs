using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Models
{
    [Table("Role")]
    public class Role : BaseModel
    {
        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
