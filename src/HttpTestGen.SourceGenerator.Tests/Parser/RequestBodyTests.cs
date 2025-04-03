using Xunit;

namespace HttpTestGen.SourceGenerator.Tests.Parser;

public class RequestBodyTests
{
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
