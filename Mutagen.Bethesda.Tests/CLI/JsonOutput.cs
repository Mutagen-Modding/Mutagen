#nullable enable
using System.Text.Json;
using System.Text.Json.Serialization;
using Noggog;

namespace Mutagen.Bethesda.Tests.CLI;

public record PassthroughResult
{
    [JsonPropertyName("status")]
    public string Status { get; init; } = "pass";

    [JsonPropertyName("elapsedMs")]
    public long ElapsedMs { get; init; }

    [JsonPropertyName("cacheFolder")]
    public string? CacheFolder { get; init; }

    [JsonPropertyName("tests")]
    public TestResults Tests { get; init; } = new();

    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}

public record TestResults
{
    [JsonPropertyName("normal")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TestEntry? Normal { get; init; }

    [JsonPropertyName("overlay")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TestEntry? Overlay { get; init; }

    [JsonPropertyName("copyin")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TestEntry? CopyIn { get; init; }
}

public record TestEntry
{
    [JsonPropertyName("status")]
    public string Status { get; init; } = "skip";

    [JsonPropertyName("elapsedMs")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? ElapsedMs { get; init; }

    [JsonPropertyName("error")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ErrorInfo? Error { get; init; }
}

public record ErrorInfo
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    [JsonPropertyName("position")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Position { get; init; }

    [JsonPropertyName("positions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Positions { get; init; }

    [JsonPropertyName("file")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? File { get; init; }

    [JsonPropertyName("message")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; init; }

    [JsonPropertyName("exceptionType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ExceptionType { get; init; }

    public static ErrorInfo FromException(Exception ex)
    {
        return ex switch
        {
            DidNotMatchException didNotMatch => new ErrorInfo
            {
                Type = "DidNotMatch",
                Positions = didNotMatch.Errors.Select(r => r.ToString("X")).ToList(),
                File = didNotMatch.Path
            },
            MoreDataException moreData => new ErrorInfo
            {
                Type = "MoreData",
                Position = $"0x{moreData.Position:X}",
                File = moreData.Path
            },
            UnexpectedlyMoreData unexpectedlyMore => new ErrorInfo
            {
                Type = "UnexpectedlyMoreData",
                Position = $"0x{unexpectedlyMore.Position:X}",
                File = unexpectedlyMore.Path
            },
            _ => new ErrorInfo
            {
                Type = "Exception",
                Message = ex.Message,
                ExceptionType = ex.GetType().FullName
            }
        };
    }
}

public record TestResult(string Name, bool Passed, Exception? Error, long ElapsedMs);
