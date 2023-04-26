namespace IrzUccApi.Models.Dtos
{
    public record EventDto : EventListItemDto
    {
        public EventDto(
            Guid id,
            string title,
            DateTime start,
            DateTime end,
            string? description,
            string? cabinetName,
            bool isPublic,
            UserHeaderDto creator,
            IEnumerable<UserHeaderDto> listeners) : base(
                id,
                title,
                start,
                end,
                cabinetName)
        {
            Description = description;
            IsPublic = isPublic;
            Creator = creator;
            Listeners = listeners;
        }

        public string? Description { get; }
        public bool IsPublic { get; }

        public UserHeaderDto Creator { get; }
        public IEnumerable<UserHeaderDto> Listeners { get; }
    }
}
