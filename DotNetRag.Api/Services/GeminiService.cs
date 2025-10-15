using System.Text;
using System.Text.Json;

namespace DotNetRag.Api.Services;
public class GeminiService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    public GeminiService(string apiKey)
    {
        _apiKey = apiKey;
        _http = new HttpClient();
        _http.DefaultRequestHeaders.Add("x-goog-api-key", _apiKey);
    }

    public async Task<float[]> GetEmbeddingAsync(string text)
    {
        var url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-embedding-001:embedContent";
        var body = new
        {
            model = "gemini-embedding-001",
            content = new { parts = new[] { new { text = text } } }
        };

        var json = JsonSerializer.Serialize(body);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var resp = await _http.PostAsync(url, content);
        var txt = await resp.Content.ReadAsStringAsync();

        // 🔍 Agrega esto para ver la respuesta completa:
        Console.WriteLine($"[Embedding Response]: {txt}");

        if (!resp.IsSuccessStatusCode)
            throw new Exception($"Error embedding: {resp.StatusCode} - {txt}");

        using var doc = JsonDocument.Parse(txt);

        // 👇 Cambia esta línea:
        var vals = doc.RootElement
            .GetProperty("embedding")
            .GetProperty("values");

        return vals.EnumerateArray().Select(v => v.GetSingle()).ToArray();
    }


    public async Task<string> AskAsync(string question, string context)
    {
        var url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";
        var body = new
        {
            contents = new[]
            {
                new {
                    parts = new[]
                    {
                        new { text = $"Contexto:\n{context}\n\nPregunta:\n{question}" }
                    }
                }
            }
        };
        var json = JsonSerializer.Serialize(body);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var resp = await _http.PostAsync(url, content);
        var txt = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
            throw new Exception($"Error Gemini: {resp.StatusCode} - {txt}");
        using var doc = JsonDocument.Parse(txt);
        var outTxt = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text").GetString();
        return outTxt ?? "";
    }
}
