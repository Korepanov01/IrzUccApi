using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Models
{
    [Table("PositionHistoricalRecord")]
    public class PositionHistoricalRecord : BaseModel
    {
        public DateTime DateTime { get; } = DateTime.UtcNow;
        public string PositionName { get; set; }

        public virtual User User { get; set; }
    }
}
