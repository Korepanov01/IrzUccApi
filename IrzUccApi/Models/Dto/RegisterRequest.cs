using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Dbo;

public class RegisterRequest
{
    [Required(AllowEmptyStrings = false)]
    [MaxLength(50)]
    public string? FirstName { get; set; }
    [Required(AllowEmptyStrings = false)]
    [MaxLength(50)]
    public string? Surname { get; set; }
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