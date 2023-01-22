using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Dtos.Position
{
    public class PositionDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; } = string.Empty;
    }
}
