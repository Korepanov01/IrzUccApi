namespace IrzUccApi.Models.Dtos
{
    public record UserHeaderDto
    {
        public UserHeaderDto(
            Guid id,
            string firstName,
            string surname,
            string? patronymic,
            string? imagePath)
        {
            Id = id;
            FirstName = firstName;
            Surname = surname;
            Patronymic = patronymic;
            ImagePath = imagePath;
        }

        public Guid Id { get; }
        public string FirstName { get; }
        public string Surname { get; }
        public string? Patronymic { get; }
        public string? ImagePath { get; }
    }
}
