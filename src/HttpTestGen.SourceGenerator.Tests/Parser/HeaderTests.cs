using Xunit;

namespace HttpTestGen.SourceGenerator.Tests.Parser;

public class HeaderTests
{
    [Theory]
    [InlineData(
        """
        GET https://localhost/ HTTP/1.1
        Accept: application/json
        Authorization: Bearer token
        x-custom-header: custom value
        """)]
    [InlineData(
        """
        #
        GET https://localhost/ HTTP/1.1
        Accept: application/json
        Authorization: Bearer token
        x-custom-header: custom value
        """)]
    [InlineData(
        """
        #
        GET https://localhost/ HTTP/1.1
        Accept: application/json
        Authorization: Bearer token
        x-custom-header: custom value
        #
        """)]
    [InlineData(
        """
        
        #

        GET https://localhost/ HTTP/1.1
        Accept: application/json
        Authorization: Bearer token
        x-custom-header: custom value
        
        
        """)]
    public void Parse_Headers_Request(string content)
    {
        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        var first = requests.Single();
        Assert.Equal("GET", first.Method);
        Assert.Equal("https://localhost/", first.Endpoint);
        Assert.True(first.Headers.ContainsKey("Accept"));
        Assert.Equal("application/json", first.Headers["Accept"]);
        Assert.Equal("Bearer token", first.Headers["Authorization"]);
        Assert.Equal("custom value", first.Headers["x-custom-header"]);
    }
}
