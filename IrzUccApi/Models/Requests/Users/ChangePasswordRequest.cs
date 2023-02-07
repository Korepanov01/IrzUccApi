using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Requests.Users
{
    public class ChangePasswordRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string CurrentPassword { get; set; } = string.Empty;
        [Required(AllowEmptyStrings = false)]
        public string NewPassword { get; set; } = string.Empty;
    }
}
