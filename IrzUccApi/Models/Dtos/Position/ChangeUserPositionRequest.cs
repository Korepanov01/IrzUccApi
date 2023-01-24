using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Dtos.Position
{
    public class ChangeUserPositionRequest
    {
        public bool IsRemoving { get; set; } = false;
        public int? PositionId { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string UserId { get; set; } = string.Empty;
    }
}
