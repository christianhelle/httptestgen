using Xunit;

namespace HttpTestGen.SourceGenerator.Tests;

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
            """
        ).ToList();

        var first = requests.First();
        Assert.Equal("GET", first.Method);
        Assert.Equal("https://localhost/", first.Endpoint);

        var second = requests.Skip(1).First();
        Assert.Equal("GET", second.Method);
        Assert.Equal("https://localhost/foo", second.Endpoint);
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

    [Fact]
    public void Parse_Body_Request()
    {
        var sut = new HttpFileParser();
        var requests = sut.Parse(
            """
            GET https://localhost/ HTTP/1.1
            Accept: application/json

            {
              "id": 1234,
              "type": "message",
              "payload": {
                "name": "test"
              }
            }
            """
        ).ToList();

        var first = requests.Single();
        Assert.Equal("GET", first.Method);
        Assert.Equal("https://localhost/", first.Endpoint);
        Assert.NotNull(first.RequestBody);
        Assert.False(string.IsNullOrWhiteSpace(first.RequestBody));
    }
}
