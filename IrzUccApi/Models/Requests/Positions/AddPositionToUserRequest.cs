using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Requests.Position
{
    public class AddPositionToUserRequest
    {
        [Required]
        public Guid PositionId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public DateTime Start { get; set; } = DateTime.UtcNow;
    }
}
