using Xunit;

namespace HttpTestGen.SourceGenerator.Tests.Parser;

public class HeaderTests
{
    [Fact]
    public void Parse_Headers_Request()
    {
        var sut = new HttpFileParser();
        var requests = sut.Parse(
            """
            GET https://localhost/ HTTP/1.1
            Accept: application/json
            Authorization: Bearer token
            x-custom-header: custom value
            """
        );

        var first = requests.Single();
        Assert.Equal("GET", first.Method);
        Assert.Equal("https://localhost/", first.Endpoint);
        Assert.True(first.Headers.ContainsKey("Accept"));
        Assert.Equal("application/json", first.Headers["Accept"]);
        Assert.Equal("Bearer token", first.Headers["Authorization"]);
        Assert.Equal("custom value", first.Headers["x-custom-header"]);
    }
}
