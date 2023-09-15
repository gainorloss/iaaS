using Dev.API.Infras;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class JwtBearerAuthenticationExtensions
    {
        public static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services, IConfigurationSection jwtSection)
        {
            services.Configure<JwtOptions>(jwtSection);

            var jwt = new JwtOptions();
            jwtSection.Bind(jwt);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
             {
                 opt.RequireHttpsMetadata = false;
                 opt.SaveToken = true;
                 opt.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = jwt.Issuer,
                     ValidAudience = jwt.Audience,
                     ValidateLifetime = true,
                     ClockSkew = System.TimeSpan.Zero
                 };
                 if (!string.IsNullOrEmpty(jwt.SecurityKey))
                     opt.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecurityKey));
             });
            services.AddScoped<JwtHelper>();
            return services;
        }
    }
}
