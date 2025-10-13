using DotNetRag.Api.Services;

namespace DotNetRag.Api.Services;
public class RagService(GeminiAIService ai, QdrantService qdrant)
{
  

    public async Task<string> AskAsync()
    {
        //var qEmbedding = await _ai.CreateEmbeddingAsync(question);
        //var contextDocs = await _qdrant.SearchAsync(qEmbedding);
        //var context = string.Join("\n", contextDocs);

        //var prompt = $"Usa el siguiente contexto para responder:\n{context}\n\nPregunta: {question}";
        //return await _ai.GenerateAnswerAsync(prompt);
        return null!;
    }
}
