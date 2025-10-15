using System.Text.Json;

public class DocumentStore
{
    private readonly string _path;
    private readonly List<Document> _docs = new();
    public DocumentStore(string path)
    {
        _path = path;
        if (File.Exists(_path))
        {
            var txt = File.ReadAllText(_path);
            _docs = JsonSerializer.Deserialize<List<Document>>(txt) ?? new List<Document>();
        }
    }
    public void Add(Document d) { _docs.Add(d); Save(); }
    public void Save()
    {
        var opts = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(_path, JsonSerializer.Serialize(_docs, opts));
    }
    public IEnumerable<(Document, float)> Search(float[] qEmb, int topK = 5)
    {
        static float Cosine(float[] a, float[] b)
        {
            double dot = 0, na = 0, nb = 0;
            int len = Math.Min(a.Length, b.Length);
            for (int i = 0; i < len; i++)
            {
                dot += a[i] * b[i];
                na += a[i] * a[i];
                nb += b[i] * b[i];
            }
            if (na == 0 || nb == 0) return 0;
            return (float)(dot / (Math.Sqrt(na) * Math.Sqrt(nb)));
        }
        return _docs.Select(d => (d, Cosine(d.Embedding, qEmb)))
                    .OrderByDescending(x => x.Item2)
                    .Take(topK);
    }
    public IEnumerable<Document> All() => _docs;
}
