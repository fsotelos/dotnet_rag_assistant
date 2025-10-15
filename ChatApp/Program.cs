using System.ClientModel;
using Microsoft.Extensions.AI;
using OpenAI;
using ChatApp.Components;
using ChatApp.Services;
using ChatApp.Services.Ingestion;
using System.Net.Http; // Make sure this is included
using OpenAI.Chat; // Include any necessary namespaces for your custom services

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// Removed OpenAI configuration as it is not needed for chat or embeddings anymore.
// You will need to make sure the Custom services are available, e.g. in the ChatApp.Services namespace.

var vectorStorePath = Path.Combine(AppContext.BaseDirectory, "vector-store1.db");
var vectorStoreConnectionString = $"Data Source={vectorStorePath}";

// --- Custom Embedding Generator Registration ---
// Use AddHttpClient<T> to register the embedding generator and its HttpClient together.
builder.Services.AddHttpClient<IEmbeddingGenerator<string, Embedding<float>>, CustomEmbeddingGenerator>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7100/api/");
});

builder.Services.AddSqliteCollection<string, IngestedChunk>("data-chatapp-chunks", vectorStoreConnectionString);
builder.Services.AddSqliteCollection<string, IngestedDocument>("data-chatapp-documents", vectorStoreConnectionString);

builder.Services.AddScoped<DataIngestor>();
builder.Services.AddSingleton<SemanticSearch>();

// --- Custom Chat Client Registration ---
builder.Services.AddChatClient(services =>
{
    var httpClientFactory = services.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("CustomChatApiClient");
    return new CustomChatService(httpClient);
}).UseFunctionInvocation().UseLogging();

builder.Services.AddHttpClient("CustomChatApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7100/api/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.UseStaticFiles();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// You should only call IngestDataAsync if you intend to run ingestion on every startup.
// You can uncomment this line if needed, but ensure your custom services work first.
//await DataIngestor.IngestDataAsync(
//    app.Services,
//    new PDFDirectorySource(Path.Combine(builder.Environment.WebRootPath, "Data")));

app.Run();
