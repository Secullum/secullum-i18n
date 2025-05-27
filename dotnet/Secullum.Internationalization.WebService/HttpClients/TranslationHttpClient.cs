using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Secullum.Internationalization.WebService.HttpClients
{
    public class TranslatorSettings
    {
        public string SubscriptionKey { get; set; }
        public string Endpoint { get; set; }
        public string Region { get; set; }
    }

    public interface ITranslationHttpClient
    {
        Task<string> TranslateAsync(string text, string targetLanguage);
    }

    public class TranslationHttpClient : ITranslationHttpClient
    {
        private readonly HttpClient _httpClient;

        public TranslationHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> TranslateAsync(string text, string targetLanguage)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            var route = $"translate?api-version=3.0&to={targetLanguage}";

            var requestBody = JsonSerializer.Serialize(new[] { new { Text = text } });

            var request = new HttpRequestMessage(HttpMethod.Post, route)
            {
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await ExtractTranslation(response);
        }

        private async Task<string> ExtractTranslation(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseBody);

            return jsonDoc.RootElement[0].GetProperty("translations")[0].GetProperty("text").GetString();
        }
    }
}
