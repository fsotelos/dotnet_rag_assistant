using System.Net.Http.Headers;
using WebChat.Client.Model;
using static WebChat.Client.Components.ChatWindow;

namespace WebChat.Client.Services
{
    public class UploadApiClient
    {
        private readonly HttpClient _http;

        public UploadApiClient()
        {
            // 👇 must point to your API URL
            _http = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7100/api/")
            };
        }

        public async Task<HttpResponseMessage> UploadFilesAsync(List<LocalFile> files, string sessionId)
        {
            using var content = new MultipartFormDataContent();

            // 🔹 Add sessionId because API expects it
            content.Add(new StringContent(sessionId), "sessionId");

            foreach (var file in files)
            {
                var fileContent = new ByteArrayContent(file.Content);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                // 🔹 key must be "files"
                content.Add(fileContent, "files", file.FileName);
            }

            // 🔹 matches your controller route [Route("api/[controller]")]
            return await _http.PostAsync("file/upload-multiple", content);
        }

        public async Task<ChatMessage> Chat(string prompt) {
            var url = $"chat/ask";
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(prompt), "question");
            var response = await _http.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return new ChatMessage { Text = result ?? "(no answer)", IsUser = false };
            }
            else
            {
                return new ChatMessage { Text = "⚠️ Error contacting API.", IsUser = false };
            }

        }
    }
}
