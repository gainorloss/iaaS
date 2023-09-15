using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSignalr(this IServiceCollection services, IHostEnvironment env)
        {
            services.AddSignalR(opt =>
            {
                opt.MaximumReceiveMessageSize = 1024 * 1024 * 1024;
                opt.EnableDetailedErrors = env.IsDevelopment();
            })//TODO:研究 messagepack
              //.AddMessagePackProtocol(opt => opt.SerializerOptions.WithResolver(ContractlessStandardResolver.Instance))
                ;

            return services;
        }
    }
}
