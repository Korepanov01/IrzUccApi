namespace IrzUccApi.Models.Dtos
{
    public class UserHeaderDto
    {
        public UserHeaderDto(
            string id,
            string firstName,
            string surname,
            string? patronymic,
            string? imageId)
        {
            Id = id;
            FirstName = firstName;
            Surname = surname;
            Patronymic = patronymic;
            ImageId = imageId;
        }

        public string Id { get; }
        public string FirstName { get; }
        public string Surname { get; }
        public string? Patronymic { get; }
        public string? ImageId { get; }
    }
}
