using System.Collections.Concurrent;

namespace DotNetRag.Api.Models
{

    public class TempFileEntry
    {
        public string FileName { get; set; } = string.Empty;
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public DateTime UploadedAt { get; set; }
        public string ExtractedText { get; set; } = string.Empty;
    }
}

