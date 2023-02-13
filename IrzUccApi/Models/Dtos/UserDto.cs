﻿namespace IrzUccApi.Models.Dtos
{
    public class UserDto
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
            int subscriptionsCount
            )
        {
            Id = id;
            FirstName = firstName;
            Surname = surname;
            Patronymic = patronymic;
            Birthday = birthday;
            ImageId = imageId;
            AboutMyself = aboutMyself;
            MyDoings = myDoings;
            Skills = skills; ;
            Positions = positions;
            Roles = roles;
            SubscribersCount = subscribersCount;
            SubscriptionsCount = subscriptionsCount;
        }

        public string Id { get; }
        public string FirstName { get; }
        public string Surname { get; }
        public string? Patronymic { get; }
        public DateTime Birthday { get; }
        public Guid? ImageId { get; }
        public string? AboutMyself { get; }
        public string? MyDoings { get; }
        public string? Skills { get; }
        public IEnumerable<PositionDto> Positions { get; }
        public IEnumerable<string> Roles { get; }
        public int SubscribersCount { get; }
        public int SubscriptionsCount { get; }
    }
}
