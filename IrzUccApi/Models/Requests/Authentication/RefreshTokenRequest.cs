using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Requests.Authentication
{
    public class RefreshTokenRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Jwt { get; set; } = string.Empty;
        [Required(AllowEmptyStrings = false)]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
