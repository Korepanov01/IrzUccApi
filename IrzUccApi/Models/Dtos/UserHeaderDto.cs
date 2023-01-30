namespace IrzUccApi.Models.Dtos
{
    public class UserHeaderDto
    {
        public UserHeaderDto(
            string id,
            string firstName,
            string surname,
            string? patronymic,
            string email,
            string? image)
        {
            Id = id;
            FirstName = firstName;
            Surname = surname;
            Patronymic = patronymic;
            Image = image;
            Email = email;
        }

        public string Id { get; }
        public string FirstName { get; }
        public string Surname { get; }
        public string Email { get; }
        public string? Patronymic { get; }
        public string? Image { get; }
    }
}
