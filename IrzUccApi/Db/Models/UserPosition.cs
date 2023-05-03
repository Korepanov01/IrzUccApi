using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Db.Models
{
    [Table("UserPosition")]
    public class UserPosition : BaseDbModel, IEntity
    {
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }

        public virtual AppUser User { get; set; } = new AppUser();
        public virtual Position Position { get; set; } = new Position();
    }
}