namespace IrzUccApi.Models.Dtos
{
    public class CabinetDto
    {
        public CabinetDto(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; }
        public string Name { get; }
    }
}
