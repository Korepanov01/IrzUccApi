namespace IrzUccApi.Models.Dtos
{
    public class UserListItemDto : UserHeaderDto
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
            IEnumerable<PositionDto> positions) : base(id, firstName, surname, patronymic, imageId)
        {
            IsActiveAccount = isActiveAccount;
            Roles = roles;
            Email = email;
            Positions = positions;
        }

        public string Email { get; }
        public bool? IsActiveAccount { get; }
        public IEnumerable<string>? Roles { get; }
        public IEnumerable<PositionDto> Positions { get; }
    }
}
