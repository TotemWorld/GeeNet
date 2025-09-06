using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace GeeNet
{
    public class GeeNetClient
    {
        private readonly GeeNetOptions _options;
        private readonly HttpClient _client;
        private readonly TokenProvider _tokenProvider;
        internal GeeNetClient(IOptions<GeeNetOptions> options, HttpClient client, TokenProvider tokenProvider)
        {
            _client = client;
            _options = options.Value ?? throw new ArgumentNullException(nameof(options), "GeeNetOptions cannot be null.");
            _tokenProvider = tokenProvider;

            _client.BaseAddress = new Uri("https://earthengine.googleapis.com/" + _options.ApiVersion + "/");

        }

        internal async Task<HttpResponseMessage> PostAsync(string endpoint, string content)
        {
            string token = await _tokenProvider.GetTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var httpContent = new StringContent(content, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(endpoint, httpContent);

            return response;
        }

        internal async Task<HttpResponseMessage> GetAsyncStream(string endpoint, string content)
        {
            string token = await _tokenProvider.GetTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var request = new HttpRequestMessage(HttpMethod.Get, endpoint)
            {
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };

            var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            return response;
        }
    }
}
