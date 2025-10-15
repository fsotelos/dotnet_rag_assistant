namespace ChatApp.Services
{
    using Microsoft.Extensions.AI;
    using System;
    using System.Net.Http;
    using System.Text;
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
            // Construct the payload for your 'ask' endpoint
            var lastText = messages
                        .Last()
                        .Contents
                        .OfType<TextContent>()   // filters only text-type contents
                        .FirstOrDefault()?.Text; // gets the first text


            var url = $"chat/ask";
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(lastText), "question");
            var response =  httpClient.PostAsync(url, content).GetAwaiter().GetResult();

           
            response.EnsureSuccessStatusCode();

            // Read and deserialize the API response
            var apiResponse =  response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            // Assuming your API returns a simple string response
            var responseText = apiResponse;

            // Yield the response back to the chat UI
            yield return new ChatResponseUpdate(ChatRole.Assistant, responseText);
        }

        // Other IChatClient methods can be left unimplemented or customized
    }

}
