namespace IrzUccApi.Models.Dtos
{
    public class UserDto
    {
        public UserDto(
            string id,
            string firstName,
            string surname,
            string? patronymic,
            DateTime birthday,
            string? image,
            string? aboutMyself,
            string? myDoings,
            string? skills,
            DateTime? employmentDate,
            PositionDto? position,
            ICollection<PositionHistoricalRecordDto> positionsHistory)
        {
            Id = id;
            FirstName = firstName;
            Surname = surname;
            Patronymic = patronymic;
            Birthday = birthday;
            Image = image;
            AboutMyself = aboutMyself;
            MyDoings = myDoings;
            Skills = skills;
            EmploymentDate = employmentDate;
            Position = position;
            PositionsHistory = positionsHistory;
        }

        public string Id { get; }
        public string FirstName { get; }
        public string Surname { get; }
        public string? Patronymic { get; }
        public DateTime Birthday { get; }
        public string? Image { get; }
        public string? AboutMyself { get; }
        public string? MyDoings { get; }
        public string? Skills { get; }
        public DateTime? EmploymentDate { get; }
        public PositionDto? Position { get; }

        public IEnumerable<PositionHistoricalRecordDto> PositionsHistory { get; }
    }
}
