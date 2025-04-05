namespace HttpTestGen.Core;

public class HttpFileAssertions
{
    public int ExpectedStatusCode { get; set; } = 200;
    public Dictionary<string,string> ExpectedHeaders { get; set; } = [];
}