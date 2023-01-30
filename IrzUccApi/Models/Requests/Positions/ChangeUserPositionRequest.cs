using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Requests.Position
{
    public class ChangeUserPositionRequest
    {
        [Required]
        public int PositionId { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
    }
}
