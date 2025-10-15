using Microsoft.Extensions.AI;
using System.Text;
using System.Text.Json;

namespace ChatApp.Services;
public class CustomEmbeddingGenerator(HttpClient httpClient) : IEmbeddingGenerator<string, Embedding<float>>
{
    public IAsyncEnumerable<Embedding<float>> GenerateStreamingAsync(IAsyncEnumerable<string> inputs, EmbeddingGenerationOptions? options = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    public Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(IEnumerable<string> values, Microsoft.Extensions.AI.EmbeddingGenerationOptions? options = null, CancellationToken cancellationToken = default)
    {
     
        var requestData = new { text = string.Join("", values) };
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(requestData),
            Encoding.UTF8,
            "application/json");

      
        var response = httpClient.PostAsync("embeddings", jsonContent, cancellationToken).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();

        var apiResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        var embeddingData = JsonSerializer.Deserialize<List<float>>(apiResponse);

        if (embeddingData == null)
        {
            throw new InvalidOperationException("Failed to deserialize embedding data from API response.");
        }

        var embedding = new Embedding<float>(embeddingData.ToArray());
        var embeddingsList = new List<Embedding<float>> { embedding };

        var result = new GeneratedEmbeddings<Embedding<float>>(embeddingsList);

        return Task.FromResult(result);
    }


    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}


