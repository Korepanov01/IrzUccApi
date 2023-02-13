namespace IrzUccApi.Models.Dtos
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
            Guid? imageId,
            IEnumerable<string>? roles,
            IEnumerable<PositionDto> positions)
        {
            Id = id;
            FirstName = firstName;
            Surname = surname;
            IsActiveAccount = isActiveAccount;
            Patronymic = patronymic;
            ImageId = imageId;
            Roles = roles;
            Email = email;
            Positions = positions;
        }

        public string Id { get; }
        public string FirstName { get; }
        public string Surname { get; }
        public string Email { get; }
        public bool? IsActiveAccount { get; }
        public string? Patronymic { get; }
        public Guid? ImageId { get; }
        public IEnumerable<string>? Roles { get; }
        public IEnumerable<PositionDto> Positions { get; }
    }
}
