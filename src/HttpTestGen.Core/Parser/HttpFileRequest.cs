using System.Collections.Specialized;

namespace HttpTestGen.Core;

public class HttpFileRequest
{
    public string? Name { get; set; }
    public string Method { get; set; } = null!;
    public string Endpoint { get; set; } = null!;
    public string? RequestBody { get; set; }
    public StringDictionary Headers { get; set; } = [];
    public HttpFileAssertions Assertions { get; set; } = new();
    public long? TimeoutMs { get; set; }
    public long? ConnectionTimeoutMs { get; set; }
    public string? DependsOn { get; set; }
    public List<HttpFileCondition> Conditions { get; set; } = new List<HttpFileCondition>();
    public long? PreDelayMs { get; set; }
    public long? PostDelayMs { get; set; }
}