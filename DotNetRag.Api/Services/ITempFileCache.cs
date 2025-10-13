using DotNetRag.Api.Models;

namespace DotNetRag.Api.Services
{
    public interface ITempFileCache
    {
        Task AddFileAsync(string sessionId, TempFileEntry entry);
        Task<List<TempFileEntry>> GetFilesAsync(string sessionId);
        Task RemoveSessionAsync(string sessionId);
    }
}

