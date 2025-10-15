using System.Text.Json.Serialization;

namespace DotNetRag.Api.Services;

public class GeminiResponse
{
    [JsonPropertyName("candidates")]
    public List<Candidate> Candidates { get; set; } = new();

    [JsonPropertyName("usageMetadata")]
    public UsageMetadata UsageMetadata { get; set; } = new();

    [JsonPropertyName("modelVersion")]
    public string ModelVersion { get; set; } = string.Empty;

    [JsonPropertyName("responseId")]
    public string ResponseId { get; set; } = string.Empty;
}

public class Candidate
{
    [JsonPropertyName("content")]
    public Content Content { get; set; } = new();

    [JsonPropertyName("finishReason")]
    public string FinishReason { get; set; } = string.Empty;

    [JsonPropertyName("index")]
    public int Index { get; set; }
}

public class Content
{
    [JsonPropertyName("parts")]
    public List<Part> Parts { get; set; } = new();

    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;
}

public class Part
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}

public class UsageMetadata
{
    [JsonPropertyName("promptTokenCount")]
    public int PromptTokenCount { get; set; }

    [JsonPropertyName("candidatesTokenCount")]
    public int CandidatesTokenCount { get; set; }

    [JsonPropertyName("totalTokenCount")]
    public int TotalTokenCount { get; set; }

    [JsonPropertyName("promptTokensDetails")]
    public List<PromptTokensDetail> PromptTokensDetails { get; set; } = new();

    [JsonPropertyName("thoughtsTokenCount")]
    public int ThoughtsTokenCount { get; set; }
}

public class PromptTokensDetail
{
    [JsonPropertyName("modality")]
    public string Modality { get; set; } = string.Empty;

    [JsonPropertyName("tokenCount")]
    public int TokenCount { get; set; }
}


