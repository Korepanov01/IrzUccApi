using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Requests.Users
{
    public class ResetPasswordRequest
    {
        [Required(AllowEmptyStrings = false), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false)]
        public string Token { get; set; } = string.Empty;
    }
}
