using DotNetRag.Api.Models;
using StackExchange.Redis;

namespace DotNetRag.Api.Services
{
    public class RedisTempFileCache(IConnectionMultiplexer redis) : ITempFileCache
    {
        private readonly IDatabase _db = redis.GetDatabase();
        private readonly string _prefix = "session_files:";

        public async Task AddFileAsync(string sessionId, TempFileEntry entry)
        {
            var key = _prefix + sessionId;
            var files = await GetFilesAsync(sessionId) ?? [];
            files.Add(entry);
            var serialized = System.Text.Json.JsonSerializer.Serialize(files);
            await _db.StringSetAsync(key, serialized);
        }

        public async Task<List<TempFileEntry>> GetFilesAsync(string sessionId)
        {
            var key = _prefix + sessionId;
            var value = await _db.StringGetAsync(key);
            if (value.IsNullOrEmpty)
                return [];
            return System.Text.Json.JsonSerializer.Deserialize<List<TempFileEntry>>(value.ToString()) ?? [];
        }

        public async Task RemoveSessionAsync(string sessionId)
        {
            var key = _prefix + sessionId;
            await _db.KeyDeleteAsync(key);
        }
    }
}