using Microsoft.Build.Framework;

namespace IrzUccApi.Models.Dtos
{
    public class Tokens
    {
        public string Jwt { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
