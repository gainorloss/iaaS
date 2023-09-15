namespace Microsoft.Extensions.DependencyInjection
{
    internal class HubConnectionBuilderOptions
        : IHubConnectionBuilderOptions
    {
        public HubConnectionBuilderOptions()
        {
        }

        public string BaseUrl { get; set; }
    }
}