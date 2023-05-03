namespace IrzUccApi.Models.Dtos
{
    public record UserDto : UserListItemDto
    {
        public UserDto(
            Guid id,
            string firstName,
            string surname,
            string? patronymic,
            DateTime birthday,
            string? imagePath,
            string? aboutMyself,
            string? myDoings,
            string? skills,
            IEnumerable<PositionDto> positions,
            IEnumerable<string> roles,
            int subscribersCount,
            int subscriptionsCount,
            string email,
            bool? isActiveAccount,
            bool isSubscription) : base(
                id,
                firstName,
                surname,
                patronymic,
                email,
                isActiveAccount,
                imagePath, roles,
                positions)
        {
            Birthday = birthday;
            AboutMyself = aboutMyself;
            MyDoings = myDoings;
            Skills = skills;
            SubscribersCount = subscribersCount;
            SubscriptionsCount = subscriptionsCount;
            IsSubscription = isSubscription;
        }

        public DateTime Birthday { get; }
        public string? AboutMyself { get; }
        public string? MyDoings { get; }
        public string? Skills { get; }
        public int SubscribersCount { get; }
        public int SubscriptionsCount { get; }
        public bool IsSubscription { get; }
    }
}
