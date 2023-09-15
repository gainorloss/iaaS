using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSwaggerGen(this IServiceCollection services, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = Assembly.GetEntryAssembly().GetName().Name;
            }

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = name,
                    Version = "v1",
                    Description = $"{name} 在线文档"
                });
                c.DocInclusionPredicate((version, desc) => true);
                c.CustomSchemaIds(type => type.FullName);
                c.IncludeXmlComments();
                c.DescribeAllParametersInCamelCase();
                c.AddSecurity();
            });

            return services;
        }

        internal static void IncludeXmlComments(this SwaggerGenOptions c)
        {
            var libs = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var lib in libs)
            {
                var path = Path.Combine(AppContext.BaseDirectory, $"{lib.GetName().Name}.xml");

                if (!File.Exists(path))
                    continue;

                c.IncludeXmlComments(path, true);
            }
        }
        internal static void AddSecurity(this SwaggerGenOptions c)
        {
            c.AddSecurityDefinition("Oauth", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Scheme = "Bearer",
                Type = SecuritySchemeType.Http,
                Name = "Authorization"
            });

            var schema = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Oauth"
                }
            };
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                [schema] = new string[0]
            });
        }
    }
}
