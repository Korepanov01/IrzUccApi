using IrzUccApi.Models.Requests.Users;
using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Requests.User;

public class RegUserRequest : UpdateRegInfoRequest
{
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}