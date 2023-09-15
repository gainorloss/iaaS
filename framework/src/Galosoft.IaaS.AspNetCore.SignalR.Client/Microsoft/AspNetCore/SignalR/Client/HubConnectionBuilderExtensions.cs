namespace Microsoft.AspNetCore.SignalR.Client
{
    public static class HubConnectionBuilderExtensions
    {
        public static IHubConnectionBuilder WithDefaultUrl(this IHubConnectionBuilder builder, string baseUrl)
        {
            builder.WithUrl($"{baseUrl}/signalr");
            return builder;
        }
    }
}
