using System.Text.Json.Serialization;

namespace Dev.API.Infras
{
    public class AuthorizationToken
    {
        public AuthorizationToken()
        { }
        public AuthorizationToken(
            string accessToken,
            double expiresIn,
            string refreshToken,
            string tokenType = "Bearer")
        {
            AccessToken = accessToken;
            ExpiresIn = expiresIn;
            RefreshToken = refreshToken;
            TokenType = tokenType;
        }

        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }
        [JsonPropertyName("expires_in")]
        public double ExpiresIn { get; set; }
        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }
    }
}
