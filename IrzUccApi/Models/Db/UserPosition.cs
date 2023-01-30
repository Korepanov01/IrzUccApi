using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Models.Db
{
    [Table("userPosition")]
    public class UserPosition
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual AppUser User { get; set; } = new AppUser();
        public virtual Position Position { get; set; } = new Position();
    }
}
