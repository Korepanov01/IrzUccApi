using Microsoft.Build.Framework;

namespace IrzUccApi.Jwt
{
    public class Tokens
    {
        [Required]
        public string? Jwt { get; set; }
        [Required]
        public string? RefreshToken { get; set; }
    }
}
