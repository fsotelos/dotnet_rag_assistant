namespace ChatApp.Services
{
    using Microsoft.Extensions.AI;
    using System;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;


    public class CustomChatService(HttpClient httpClient) : IChatClient
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
        {
            return null;
        }

        public object? GetService(Type serviceType, object? serviceKey = null)
        {
            return null;
        }

      

        public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
        {
            var lastText = messages
                        .Last()
                        .Contents
                        .OfType<TextContent>()   
                        .FirstOrDefault()?.Text; 


            var url = $"chat/ask";
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(lastText), "question");
            var response =  httpClient.PostAsync(url, content).GetAwaiter().GetResult();

           
            response.EnsureSuccessStatusCode();

            
            var apiResponse =  response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            
            var responseText = apiResponse;

            string json = responseText;

            var result = JsonSerializer.Deserialize<JsonElement>(json);

            string formattedText = result.GetProperty("answer").GetString();

            yield return new ChatResponseUpdate(ChatRole.Assistant, formattedText);
        }

        
    }

}
