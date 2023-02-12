using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Requests.Authentication
{
    public class LoginRequest
    {
        [Required(AllowEmptyStrings = false)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; } = string.Empty;
    }
}
