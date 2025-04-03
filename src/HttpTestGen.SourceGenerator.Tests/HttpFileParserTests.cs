using HttpTestGen.SourceGenerator;
using Xunit;

namespace HttpTestGen.Tests;

public class HttpFileParserTests
{
    [Fact]
    public void Parse_Single_Request()
    {
        var sut = new HttpFileParser();
        var requests = sut.Parse("GET / HTTP/1.1");

        var result = requests.Single();
        Assert.Equal("GET", result.Method);
        Assert.Equal("/", result.Endpoint);
        Assert.Equal("HTTP/1.1", result.HttpVersion);
    }

    [Fact]
    public void Parse_Multiple_Request()
    {
        var sut = new HttpFileParser();
        var requests = sut.Parse(
            """
            GET https://localhost/ HTTP/1.1
            #

            GET https://localhost/foo HTTP/1.1
            #
            """);

        var first = requests.First();
        Assert.Equal("GET", first.Method);
        Assert.Equal("https://localhost/", first.Endpoint);
        Assert.Equal("HTTP/1.1", first.HttpVersion);

        var second = requests.Skip(1).First();
        Assert.Equal("GET", second.Method);
        Assert.Equal("https://localhost/foo", second.Endpoint);
        Assert.Equal("HTTP/1.1", second.HttpVersion);
    }

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
            """);

        var first = requests.Single();
        Assert.Equal("GET", first.Method);
        Assert.Equal("https://localhost/", first.Endpoint);
        Assert.Equal("HTTP/1.1", first.HttpVersion);
        Assert.True(first.Headers.ContainsKey("Accept"));
        Assert.Equal("application/json", first.Headers["Accept"]);
        Assert.Equal("Bearer token", first.Headers["Authorization"]);
        Assert.Equal("custom value", first.Headers["x-custom-header"]);
    }
}