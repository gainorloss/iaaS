using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Dev.API.Infras
{

    public class JwtHelper
    {
        private readonly JwtOptions jwtSetting;
        public JwtHelper(IOptionsMonitor<JwtOptions> optionsAccessor)
        {
            jwtSetting = optionsAccessor.CurrentValue;
        }

        public AuthorizationToken GenerateToken(ClaimsIdentity identity)
        {
            //if (identity is null)
            //    throw new ArgumentNullException(nameof(identity));

            if (jwtSetting is null)
                throw new ArgumentNullException(nameof(jwtSetting));

            var tokenHandler = new JwtSecurityTokenHandler();

            var descriptor = new SecurityTokenDescriptor()
            {
                Audience = jwtSetting.Audience,
                Issuer = jwtSetting.Issuer,
                Expires = DateTime.UtcNow.AddSeconds(jwtSetting.ExpiresIn),
                Subject = identity
            };

            if (!string.IsNullOrEmpty(jwtSetting.SecurityKey))
                descriptor.SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.SecurityKey)), SecurityAlgorithms.HmacSha256);

            var securityToken = tokenHandler.CreateToken(descriptor);
            var jwt = tokenHandler.WriteToken(securityToken);

            var refreshToken = new RefreshToken
            {
                JwtId = securityToken.Id,
                ExpiredAt = DateTime.UtcNow.AddMonths(6),
                Token = GenerateRandomNumber()
            };
            return new AuthorizationToken(jwt, jwtSetting.ExpiresIn, refreshToken.Token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidAudience = jwtSetting.Audience,
                ValidIssuer = jwtSetting.Issuer,
                ClockSkew = TimeSpan.Zero
            };

            if (!string.IsNullOrEmpty(jwtSetting.SecurityKey))
                tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.SecurityKey));

            var securityTokenHandler = new JwtSecurityTokenHandler();

            var principal = securityTokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            var algorithmValidated = securityToken is JwtSecurityToken jwtSecurityToken && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
            return algorithmValidated ? principal : null;
        }

        private string GenerateRandomNumber(int len = 32)
        {
            var randomNumber = new byte[len];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
