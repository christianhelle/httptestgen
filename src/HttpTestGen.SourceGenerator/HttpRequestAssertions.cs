using System.Collections.Specialized;

namespace HttpTestGen.SourceGenerator;

public class HttpRequestAssertions
{
    public int ExpectedStatusCode { get; set; } = 200;
    public Dictionary<string,string> ExpectedHeaders { get; set; } = [];
}