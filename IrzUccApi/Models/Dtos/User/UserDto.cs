using IrzUccApi.Models.Dtos.Position;
using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Dtos.User
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
            string? positionName,
            ICollection<string> positionsHistory)
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
            PositionName = positionName;
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
        public string? PositionName { get; }

        public IEnumerable<string> PositionsHistory { get; }
    }
}
