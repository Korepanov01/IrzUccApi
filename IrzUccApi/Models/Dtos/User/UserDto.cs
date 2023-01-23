using IrzUccApi.Models.Dtos.Position;
using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Dtos.User
{
    public class UserDto
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? Surname { get; set; }
        public bool? IsActiveAccount { get; set; }
        public string? Patronymic { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Image { get; set; }
        public string? AboutMyself { get; set; }
        public string? MyDoings { get; set; }
        public string? Skills { get; set; }
        public IEnumerable<string>? Roles { get; set; }

        public DateTime? EmploymentDate { get; set; }
        public string? PositionName { get; set; }
        public int? PositionId { get; set; }
    }
}
