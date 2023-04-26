namespace IrzUccApi.Models.Dtos
{
    public record TokensDto
    {
        public TokensDto(string jwt, string refreshToken)
        {
            Jwt = jwt;
            RefreshToken = refreshToken;
        }

        public string Jwt { get; }
        public string RefreshToken { get; }
    }
}
