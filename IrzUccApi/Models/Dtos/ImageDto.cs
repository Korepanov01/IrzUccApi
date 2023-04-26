namespace IrzUccApi.Models.Dtos
{
    public record ImageDto
    {
        public ImageDto(
            Guid id,
            string name,
            string extension,
            string data)
        {
            Id = id;
            Name = name;
            Extension = extension;
            Data = data;
        }

        public Guid Id { get; }
        public string Name { get; }
        public string Extension { get; }
        public string Data { get; }
    }
}
