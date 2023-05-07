using IrzUccApi.Db.Models;

namespace IrzUccApi.Models.Dtos
{
    public record UserDto : UserListItemDto
    {
        public UserDto(AppUser user, AppUser currentUser) : this(
            user.Id,
            user.FirstName,
            user.Surname,
            user.Patronymic,
            user.Birthday,
            user.Image?.Id,
            user.AboutMyself,
            user.MyDoings,
            user.Skills,
            user.UserPosition
                .Where(up => up.End == null)
                .Select(up => new PositionDto(
                    up.Position.Id,
                    up.Position.Name)),
            user.UserRoles.Select(ur => ur.Role != null ? ur.Role.Name : ""),
            user.Subscribers.Count,
            user.Subscriptions.Count,
            user.Email,
            user.IsActiveAccount,
            user.Subscribers.Contains(currentUser)) { }

        public UserDto(
            Guid id,
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
            bool? isActiveAccount,
            bool isSubscription) : base(
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
