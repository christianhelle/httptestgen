using Xunit;

namespace HttpTestGen.SourceGenerator.Tests.Parser;

public class RequestBodyTests
{
    [Theory]
    [InlineData(
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
    )]
    [InlineData(
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
    )]
    [InlineData(
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
    )]
    [InlineData(
        """
        #
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
    )]
    [InlineData(
        """
        ##
        #
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
    )]
    [InlineData(
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
        #
        """
    )]
    [InlineData(
        """
        #

        #

        GET https://localhost/ HTTP/1.1
        Accept: application/json
        
        {
          "id": 1234,
          "type": "message",
          "payload": {
            "name": "test"
          }
        }
        #
        """
    )]
    public void Parse_Body_Request(string content)
    {
        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        var first = requests.Single();
        Assert.Equal("GET", first.Method);
        Assert.Equal("https://localhost/", first.Endpoint);
        Assert.NotNull(first.RequestBody);
        Assert.False(string.IsNullOrWhiteSpace(first.RequestBody));
    }
}
