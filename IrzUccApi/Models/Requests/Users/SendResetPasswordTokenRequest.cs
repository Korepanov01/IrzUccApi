using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Requests.Users
{
    public class SendResetPasswordTokenRequest
    {
        [Required(AllowEmptyStrings = false), EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
