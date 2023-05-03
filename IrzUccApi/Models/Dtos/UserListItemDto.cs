namespace IrzUccApi.Models.Dtos
{
    public record UserListItemDto : UserHeaderDto
    {
        public UserListItemDto(
            Guid id,
            string firstName,
            string surname,
            string? patronymic,
            string email,
            bool? isActiveAccount,
            string? imagePath,
            IEnumerable<string>? roles,
            IEnumerable<PositionDto> positions) : base(id, firstName, surname, patronymic, imagePath)
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
