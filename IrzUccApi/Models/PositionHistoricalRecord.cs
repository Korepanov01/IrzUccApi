using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Models
{
    [Table("PositionHistoricalRecord")]
    public class PositionHistoricalRecord
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
        public string PositionName { get; set; } = string.Empty;

        public virtual AppUser User { get; set; } = new AppUser();
    }
}
