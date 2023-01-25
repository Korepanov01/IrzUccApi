using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Dtos.User;

public class UserRegInfo
{
    [Required(AllowEmptyStrings = false)]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;
    [Required(AllowEmptyStrings = false)]
    [MaxLength(50)]
    public string Surname { get; set; } = string.Empty;
    [MaxLength(50)]
    public string? Patronymic { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; } = string.Empty;
    [Required]
    public DateTime Birthday { get; set; }
}