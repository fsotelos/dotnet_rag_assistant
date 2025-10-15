using DotNetRag.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var geminiAiKey = builder.Configuration["GEMINI_API_KEY"];
var url = builder.Configuration.GetSection("aiServiceUrl").Value ?? string.Empty;
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor",
        policy => policy
            .AllowAnyOrigin()      // for development only
            .AllowAnyMethod()
            .AllowAnyHeader());
});
if (string.IsNullOrWhiteSpace(geminiAiKey))
{
    throw new InvalidOperationException("GEMINI_API_KEY environment variable is not set.");
}
var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
 
builder.Services.AddTransient(_ => new GeminiService(geminiAiKey));
builder.Services.AddTransient(_ => new DocumentStore("vector_store.json"));
builder.Services.AddTransient<RagPdfGemini>();

builder.Services.AddControllers();

var app = builder.Build();
app.UseCors("AllowBlazor");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();