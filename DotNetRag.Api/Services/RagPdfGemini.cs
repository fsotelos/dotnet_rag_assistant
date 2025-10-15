public class RagPdfGemini
{
    private readonly GeminiService _gem;
    private readonly DocumentStore _store;
    private const int ChunkSize = 800; // caracteres por chunk
    private const int TopK = 5;

    public RagPdfGemini(GeminiService g, DocumentStore s) { _gem = g; _store = s; }

    public static IEnumerable<string> SplitText(string text, int size = ChunkSize)
    {
        for (int i = 0; i < text.Length; i += size)
            yield return text.Substring(i, Math.Min(size, text.Length - i));
    }

    public async Task LoadPdfAndIndex(string text)
    {
       var chunks = SplitText(text).ToList();
        Console.WriteLine($"Total chunks: {chunks.Count}");

        foreach (var chunk in chunks)
        {
            var emb = await _gem.GetEmbeddingAsync(chunk);
            _store.Add(new Document(Guid.NewGuid().ToString(), chunk, emb));
        }
        Console.WriteLine("PDF indexed successfull ✅");
    }

    public async Task GetEmbeddings(string text)
    {
                var emb = await _gem.GetEmbeddingAsync(text);
        Console.WriteLine($"Embedding length: {emb.Length}");
    }
    public async Task<string> AskAsync(string question)
    {
        var qEmb = await _gem.GetEmbeddingAsync(question);
        var neighbors = _store.Search(qEmb, TopK).ToList();
        var ctx = string.Join("\n---\n", neighbors.Select(n => n.Item1.Text));
        return await _gem.AskAsync(question, ctx);
    }
}
