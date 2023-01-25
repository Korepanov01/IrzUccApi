namespace IrzUccApi.Models.Dtos.User
{
    public class UserListItemDto
    {
        public UserListItemDto(
            string id, 
            string firstName, 
            string surname,
            string? patronymic,
            string email,
            bool? isActiveAccount,
            string? image, 
            IEnumerable<string>? roles, 
            string? positionName)
        {
            Id = id;
            FirstName = firstName;
            Surname = surname;
            IsActiveAccount = isActiveAccount;
            Patronymic = patronymic;
            Image = image;
            Roles = roles;
            PositionName = positionName;
            Email = email;
        }

        public string Id { get; }
        public string FirstName { get; }
        public string Surname { get; }
        public string Email { get; }
        public bool? IsActiveAccount { get; }
        public string? Patronymic { get; }
        public string? Image { get; }
        public IEnumerable<string>? Roles { get; }
        public string? PositionName { get; }
    }
}
