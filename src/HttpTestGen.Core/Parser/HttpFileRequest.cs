using System.Collections.Specialized;

namespace HttpTestGen.Core;

public class HttpFileRequest
{
    public string Method { get; set; } = null!;
    public string Endpoint { get; set; } = null!;
    public string? RequestBody { get; set; }
    public StringDictionary Headers { get; set; } = [];
    public HttpFileAssertions Assertions { get; set; } = new();
}