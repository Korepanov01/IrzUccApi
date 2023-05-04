using IrzUccApi.Db.Models;

namespace IrzUccApi.Models.Dtos
{
    public record PositionDto
    {
        public PositionDto(Position position) : this(position.Id, position.Name) { }

        public PositionDto(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; }
        public string Name { get; }
    }
}
