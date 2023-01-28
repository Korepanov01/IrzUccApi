using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Requests.Position
{
    public class UpdatePositionRequest
    {
        [Required]
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
