namespace ChatApp.Services
{
    using Microsoft.Extensions.AI;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading;



    public class CustomEmbeddingGenerator(HttpClient httpClient) : IEmbeddingGenerator<string, Embedding<float>>
    {
        

        // You can choose to implement the streaming method or not depending on your needs.
        public IAsyncEnumerable<Embedding<float>> GenerateStreamingAsync(IAsyncEnumerable<string> inputs, EmbeddingGenerationOptions? options = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(IEnumerable<string> values, Microsoft.Extensions.AI.EmbeddingGenerationOptions? options = null, CancellationToken cancellationToken = default)
        {
            // Construct the payload for your custom embeddings API endpoint
            var requestData = new { text = string.Join("", values) };
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(requestData),
                Encoding.UTF8,
                "application/json");

            // Make the HTTP POST request to your API
            var response = httpClient.PostAsync("embeddings", jsonContent, cancellationToken).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            // Read and deserialize the API response.
            var apiResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var embeddingData = JsonSerializer.Deserialize<List<float>>(apiResponse);

            if (embeddingData == null)
            {
                throw new InvalidOperationException("Failed to deserialize embedding data from API response.");
            }

            // Correctly create the embedding object.
            var embedding = new Embedding<float>(embeddingData.ToArray());
            var embeddingsList = new List<Embedding<float>> { embedding };

            // Create the final result object.
            var result = new GeneratedEmbeddings<Embedding<float>>(embeddingsList);

            // Use Task.FromResult to wrap the result in a completed Task.
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

}
