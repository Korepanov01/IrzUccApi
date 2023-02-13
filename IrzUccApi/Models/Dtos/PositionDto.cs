namespace IrzUccApi.Models.Dtos
{
    public class PositionDto
    {
        public PositionDto(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; }
        public string Name { get; }
    }
}
