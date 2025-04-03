using Xunit;

namespace HttpTestGen.SourceGenerator.Tests.Parser;

public class SingleRequestTests
{
    [Theory]
    [InlineData("GET")]
    [InlineData("POST")]
    [InlineData("PUT")]
    [InlineData("DELETE")]
    [InlineData(" GET")]
    [InlineData(" POST")]
    [InlineData(" PUT")]
    [InlineData(" DELETE")]
    [InlineData("  GET")]
    [InlineData("  POST")]
    [InlineData("  PUT")]
    [InlineData("  DELETE")]
    [InlineData("GET ")]
    [InlineData("POST ")]
    [InlineData("PUT ")]
    [InlineData("DELETE ")]
    [InlineData("GET  ")]
    [InlineData("POST  ")]
    [InlineData("PUT  ")]
    [InlineData("DELETE  ")]
    [InlineData(" GET ")]
    [InlineData(" POST ")]
    [InlineData(" PUT ")]
    [InlineData(" DELETE ")]
    [InlineData("  GET  ")]
    [InlineData("  POST  ")]
    [InlineData("  PUT  ")]
    [InlineData("  DELETE  ")]
    public void Parse_Verb(string verb)
    {
        var sut = new HttpFileParser();
        var requests = sut.Parse($"{verb} /");

        var result = requests.Single();
        Assert.Equal(verb.Trim(), result.Method);
    }

    [Theory]
    [InlineData("GET /")]
    [InlineData("GET  /")]
    [InlineData(" GET  /")]
    [InlineData(" GET / ")]
    [InlineData("GET / ")]
    [InlineData("POST /")]
    [InlineData(" POST  /")]
    [InlineData("POST  /")]
    [InlineData("POST / ")]
    [InlineData("PUT /")]
    [InlineData(" PUT  /")]
    [InlineData("PUT  /")]
    [InlineData("PUT / ")]
    [InlineData("DELETE /")]
    [InlineData(" DELETE  /")]
    [InlineData("DELETE  /")]
    [InlineData("DELETE / ")]
    public void Parse_Endpoint(string content)
    {
        var sut = new HttpFileParser();
        var requests = sut.Parse(content);

        var result = requests.Single();
        Assert.Equal("/", result.Endpoint);
    }
}
