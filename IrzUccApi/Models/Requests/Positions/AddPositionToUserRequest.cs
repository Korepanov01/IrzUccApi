using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Requests.Position
{
    public class AddPositionToUserRequest
    {
        [Required]
        public int PositionId { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
        public DateTime Start { get; set; } = DateTime.UtcNow;
    }
}
