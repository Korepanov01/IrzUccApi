using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Dtos.Position
{
    public class PositionDto
    {
        public PositionDto(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; }
        public string Name { get; }
    }
}
