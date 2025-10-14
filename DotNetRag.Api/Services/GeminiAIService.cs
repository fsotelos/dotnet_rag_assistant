using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace DotNetRag.Api.Services
{
    public class GeminiAIService(string aiUrl, string apiKey)
    {
        private readonly HttpClient _httpClient = new();

        public async Task<string?> GenerateContentAsync(string prompt)
        {
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var requestJson = JsonSerializer.Serialize(requestBody);
            using var request = new HttpRequestMessage(HttpMethod.Post, aiUrl)
            {
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("x-goog-api-key", apiKey);

            using var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            var responseJson = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonSerializer.Deserialize<GeminiAIResponse>(responseJson);

            return geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
        }


        public async Task<float[]> GetEmbeddingAsync(string text)
        {
            var url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-embedding-001:embedContent";

            var payload = new
            {
                model = "models/gemini-embedding-001",
                content = new
                {
                    parts = new[]
                    {
                new { text }
            }
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };

            // Cambia aquí: Gemini usa "x-goog-api-key" en lugar de Authorization Bearer
            request.Headers.Add("x-goog-api-key", apiKey);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            // El campo correcto en la respuesta es: embeddings[0].values
            var vector = doc.RootElement
                .GetProperty("embedding")
                .GetProperty("values")
                .EnumerateArray()
                .Select(x => x.GetSingle())
                .ToArray();

            return vector;
        }
    }
    
}
