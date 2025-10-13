using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace DotNetRag.Api.Services
{
    public class QdrantService
    {
        private readonly QdrantClient _client;
        private const string CollectionName = "documents";
        public QdrantService(string endpoint)
        {
            _client = new QdrantClient(endpoint);
        }

        public async Task InitializeAsync()
        {
            await _client.CreateCollectionAsync(CollectionName);
        }

        public async Task StoreVectorAsync(string text, float[] embeddings)
        {
            var point = new PointStruct
            {
                Id = new PointId { Uuid = Guid.NewGuid().ToString() },
                Vectors = new Vectors { Vector = new Vector() },
                Payload = { { "text", Value.Parser.ParseJson(text) } }
            };

            await _client.UpsertAsync(CollectionName, new[] { point });
        }

        public async Task<List<string>> SearchAsync(float[] queryEmbedding, ulong topK = 3)
        {
            var results = await _client.SearchAsync(CollectionName, queryEmbedding, limit: topK);
            return results.Select(r => r.Payload["text"].StringValue).ToList();
        }
    }
}
