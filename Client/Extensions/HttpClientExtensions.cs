using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Netsphere.Client.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient client, string endpoint, T model)
        {
            var peerAsJson = JsonSerializer.Serialize(model);
            var content = new StringContent(peerAsJson, Encoding.UTF8, "application/json");
            var request = await client.PostAsync(endpoint, content);
            
            return request;
        }
    }
}
