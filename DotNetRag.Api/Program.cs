using DotNetRag.Api.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var geminiAiKey = builder.Configuration["GEMINI_API_KEY"];
var url = builder.Configuration.GetSection("aiServiceUrl").Value ?? string.Empty;

if (string.IsNullOrWhiteSpace(geminiAiKey))
{
    throw new InvalidOperationException("GEMINI_API_KEY environment variable is not set.");
}
var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisConnectionString));
 
builder.Services.AddTransient(_ => new GeminiAIService(url, geminiAiKey));
builder.Services.AddSingleton<ITempFileCache, RedisTempFileCache>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();