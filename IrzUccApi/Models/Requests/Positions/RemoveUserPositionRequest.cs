using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Requests.Positions
{
    public class RemoveUserPositionRequest
    {
        [Required]
        public int PositionId { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
        public DateTime End { get; set; } = DateTime.UtcNow;
    }
}
