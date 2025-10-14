using DotNetRag.Api.Utils;

namespace DotNetRag.Api.Services;
public class RagService(GeminiAIService ai, RedisService cache)
{
    public async Task<string> LoadInfoAsync(string text)
    {
        var chunks = TextChunker.SplitTextIntoWordChunks(text, 800);

        foreach (var (index, chunk) in chunks.Select((c, i) => (i, c)))
        {
            var embedding = await ai.GetEmbeddingAsync(chunk);
            await cache.SaveDocumentAsync($"doc:{index}", chunk, embedding);
        }
        return null!;
    }

    public async Task<string?> MakeQuestion(string question) {
        var embedding = await ai.GetEmbeddingAsync(question);
        var doc = await cache.GetSimilarDocumentsAsync(embedding, 3);
        var prompt = $"Use the next context to response:\n{doc.MinBy(x=>x.Score).Content}\n\nQuestion: {question}";
        return await ai.GenerateContentAsync(prompt);
    }

}
