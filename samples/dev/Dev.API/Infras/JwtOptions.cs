namespace Dev.API.Infras
{
    public class JwtOptions
    {
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string? SecurityKey { get; set; }
        public int ExpiresIn { get; set; }
    }
}
