using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Requests.User
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
