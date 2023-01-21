using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Dto;

public class RegisterRequest
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
    public string? Email { get; set; }
    [Required]
    [MinLength(6)]
    public string? Password { get; set; }
    [Required]
    public DateTime Birthday { get; set; }
    [Required]
    public DateTime EmploymentDate { get; set; }
}