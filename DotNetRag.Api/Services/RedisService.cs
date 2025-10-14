using StackExchange.Redis;
using System.Text.Json;

public class RedisService
{
    private readonly IDatabase _db;
    private readonly IConnectionMultiplexer _redis;

    public RedisService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _db = redis.GetDatabase();
    }

    // ✅ Guarda el documento y su embedding
    public async Task SaveDocumentAsync(string id, string content, float[] embedding)
    {
        var embeddingJson = JsonSerializer.Serialize(embedding);
        await _db.HashSetAsync($"doc:{id}", new HashEntry[]
        {
            new("content", content),
            new("embedding", embeddingJson)
        });
    }

    // ✅ Busca los documentos más parecidos
    public async Task<List<Document>> GetSimilarDocumentsAsync(float[] queryEmbedding, int topK)
    {
        var results = new List<Document>();
        var server = _redis.GetServer(_redis.GetEndPoints().First());

        // Obtener todas las keys que empiecen con "doc:"
        foreach (var key in server.Keys(pattern: "doc:*"))
        {
            var entries = await _db.HashGetAllAsync(key);
            var content = entries.FirstOrDefault(e => e.Name == "content").Value.ToString();
            var embeddingJson = entries.FirstOrDefault(e => e.Name == "embedding").Value.ToString();
            if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(embeddingJson)) continue;

            var embedding = JsonSerializer.Deserialize<float[]>(embeddingJson);
            var similarity = CosineSimilarity(queryEmbedding, embedding);

            results.Add(new Document
            {
                Id = key.ToString(),
                Content = content,
                Embedding = embedding,
                Score = similarity
            });
        }

        // Ordenar por similitud descendente
        return results.OrderByDescending(d => d.Score)
                      .Take(topK)
                      .ToList();
    }

    private static double CosineSimilarity(float[] v1, float[] v2)
    {
        var dot = v1.Zip(v2, (a, b) => a * b).Sum();
        var normA = Math.Sqrt(v1.Sum(a => a * a));
        var normB = Math.Sqrt(v2.Sum(b => b * b));
        return dot / (normA * normB);
    }
}

public class Document
{
    public string Id { get; set; }
    public string Content { get; set; }
    public float[] Embedding { get; set; }
    public double Score { get; set; }
}
