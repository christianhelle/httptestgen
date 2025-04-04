using System.Collections.Specialized;

namespace HttpTestGen.SourceGenerator;

public class HttpRequestAssertions
{
    public int ExpectedStatusCode { get; set; } = 200;
    public StringDictionary ExpectedHeaders { get; set; } = [];
}