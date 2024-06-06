using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using NAudio.Wave;
using StarCitizen_Ai_CoPilot.Core.Services;

namespace StarCitizen_Ai_CoPilot.Infrastructure
{
    public class WhisperTranscriptionService : IAudioTranscriptionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public WhisperTranscriptionService()
        {
            _httpClient = new HttpClient();
            _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? throw new InvalidOperationException("API key not found.");
        }

        public async Task<string> TransformAudioToTextAsync(byte[] audioData)
        {
            try
            {
                using MemoryStream memoryStream = new MemoryStream(audioData);
                using WaveFileReader waveFileReader = new WaveFileReader(memoryStream);

                if (waveFileReader.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
                {
                    throw new InvalidOperationException("Invalid wave format. Only PCM format is supported.");
                }

                var fileContent = new ByteArrayContent(audioData);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");

                var formData = new MultipartFormDataContent
                {
                    { fileContent, "file", "audio.wav" },
                    { new StringContent("whisper-1"), "model" }
                };

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://api.openai.com/v1/audio/transcriptions"),
                    Headers =
                    {
                        { "Authorization", $"Bearer {_apiKey}" }
                    },
                    Content = formData
                };

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<WhisperResponse>(responseBody);

                Console.WriteLine($"Response: {responseBody}");
                Console.WriteLine($"Text: {result?.text ?? string.Empty}");
                return result?.text ?? string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        private class WhisperResponse
        {
            public string? text { get; set; }
        }
    }
}