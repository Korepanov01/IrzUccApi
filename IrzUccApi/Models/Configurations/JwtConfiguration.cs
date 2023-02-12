namespace IrzUccApi.Models.Configurations
{
    public class JwtConfiguration
    {
        public string SecurityKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}
