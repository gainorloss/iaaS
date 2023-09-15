using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;

namespace Microsoft.AspNetCore.Builder
{
    public static class EndpointRoutingBuilderExtensions
    {
        public static HubEndpointConventionBuilder MapSignalrHub<T>(this IEndpointRouteBuilder endpoints)
            where T : Hub
        {
            return endpoints.MapHub<T>("/signalr");
        }
    }
}
