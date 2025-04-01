using System;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Secullum.Internationalization.WebService.Services
{
    public class TranslatorSettings
    {
        public string SubscriptionKey { get; set; }
        public string Endpoint { get; set; }
        public string Region { get; set; }
    }

    public class TranslationRequestService
    {
        private readonly HttpClient _httpClient;
        private readonly string _subscriptionKey;
        private readonly string _endpoint;
        private readonly string _region;

        public TranslationRequestService(IOptions<TranslatorSettings> settings)
        {
            _httpClient = new HttpClient();
            _subscriptionKey = settings.Value.SubscriptionKey;
            _endpoint = settings.Value.Endpoint;
            _region = settings.Value.Region;
        }

        public async Task<string> TranslateAsync(string text, string targetLanguage)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            var route = $"translate?api-version=3.0&to={targetLanguage}";

            var requestBody = JsonSerializer.Serialize(new object[]
            {
                new { Text = text }
            });

            using var request = CreateRequest(route, requestBody);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await ExtractTranslation(response);
        }

        private HttpRequestMessage CreateRequest(string route, string requestBody)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_endpoint + route),
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
            request.Headers.Add("Ocp-Apim-Subscription-Region", _region);

            return request;
        }

        private async Task<string> ExtractTranslation(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseBody);

            return jsonDoc.RootElement[0].GetProperty("translations")[0].GetProperty("text").GetString();
        }
    }
}
