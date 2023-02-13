namespace IrzUccApi.Models.Dtos
{
    public class UserDto : UserListItemDto
    {
        public UserDto(
            string id,
            string firstName,
            string surname,
            string? patronymic,
            DateTime birthday,
            Guid? imageId,
            string? aboutMyself,
            string? myDoings,
            string? skills,
            IEnumerable<PositionDto> positions,
            IEnumerable<string> roles,
            int subscribersCount,
            int subscriptionsCount,
            string email,
            bool? isActiveAccount) : base(
                id, 
                firstName, 
                surname, 
                patronymic, 
                email, 
                isActiveAccount, 
                imageId, roles, 
                positions)
        {
            Birthday = birthday;
            AboutMyself = aboutMyself;
            MyDoings = myDoings;
            Skills = skills;
            SubscribersCount = subscribersCount;
            SubscriptionsCount = subscriptionsCount;
        }

        public DateTime Birthday { get; }
        public string? AboutMyself { get; }
        public string? MyDoings { get; }
        public string? Skills { get; }
        public int SubscribersCount { get; }
        public int SubscriptionsCount { get; }
    }
}
