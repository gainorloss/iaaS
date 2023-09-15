using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Galosoft.IaaS.ServiceProxy.Http
{
    public class RestClient
    {
        private readonly HttpClient _client;

        public RestClient(HttpClient httpClient)
        {
            _client = httpClient;
        }

        public async Task<T> PostAsync<T>(string requestUri, object value)
        {
            var responseMessage = await _client.PostAsJsonAsync(requestUri, value);
            var responseJsonString = await responseMessage.Content.ReadAsStringAsync();

            //TODO: 需要解决原类型如 (int, string) 的处理
            var rt = JsonSerializer.Deserialize<T>(responseJsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return rt;
        }

        public async Task<T> GetAsync<T>(string requestUri) => await _client.GetFromJsonAsync<T>(requestUri);

    }
}
