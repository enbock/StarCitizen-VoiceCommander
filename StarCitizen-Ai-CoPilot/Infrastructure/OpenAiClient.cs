using System.Net.Http;
using System.Threading.Tasks;
using System;

namespace StarCitizen_Ai_CoPilot.Infrastructure
{
    public class OpenAiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OpenAiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? throw new InvalidOperationException("API key not found.");
        }

        public async Task<string> GetResponseAsync(string prompt)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://api.openai.com/v1/engines/davinci-codex/completions"),
                Headers =
                {
                    { "Authorization", $"Bearer {_apiKey}" }
                },
                Content = new StringContent($"{{\"prompt\":\"{prompt}\",\"max_tokens\":100}}", System.Text.Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }
    }
}
