using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Dtos.Position
{
    public class PositionName
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
