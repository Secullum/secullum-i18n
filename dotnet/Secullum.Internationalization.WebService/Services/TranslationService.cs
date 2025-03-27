using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class TranslationService
{
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly string _subscriptionKey = "<your-translator-key>";
    private readonly string _endpoint = "https://api.cognitive.microsofttranslator.com/";
    private readonly string _region = "<your-resource-location>";

    public async Task<string> TranslateAsync(string text, string targetLanguage)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        var route = $"translate?api-version=3.0&to={targetLanguage}";

        var requestBody = JsonSerializer.Serialize(new object[]
        {
            new { Text = text }
        });

        using var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(_endpoint + route),
            Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
        };

        request.Headers.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
        request.Headers.Add("Ocp-Apim-Subscription-Region", _region);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(responseBody);
        var translatedText = jsonDoc.RootElement[0].GetProperty("translations")[0].GetProperty("text").GetString();

        return translatedText;
    }
}
