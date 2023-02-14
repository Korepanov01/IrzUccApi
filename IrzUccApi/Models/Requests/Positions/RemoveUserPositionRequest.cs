using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Requests.Positions
{
    public class RemoveUserPositionRequest
    {
        [Required]
        public Guid PositionId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public DateTime End { get; set; } = DateTime.UtcNow;
    }
}
