using System.Collections.Specialized;

namespace HttpTestGen.SourceGenerator;

public class HttpFileRequest
{
    public string Method { get; set; } = null!;
    public string Endpoint { get; set; } = null!;
    public string? RequestBody { get; set; }
    public StringDictionary Headers { get; set; } = [];
    public HttpRequestAssertions Assertions { get; set; } = new();
}