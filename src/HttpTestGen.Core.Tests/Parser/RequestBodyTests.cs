

using HttpTestGen.Core;

namespace HttpTestGen.Core.Tests.Parser;

public class RequestBodyTests
{
    [Test]
    [Arguments(
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
    [Arguments(
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
    [Arguments(
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
    [Arguments(
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
    [Arguments(
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
    [Arguments(
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
    [Arguments(
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
    public async Task Parse_Body_Request(string content)
    {
        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        var first = requests.Single();
        await Assert.That(first.Method).IsEqualTo("GET");
        await Assert.That(first.Endpoint).IsEqualTo("https://localhost/");
        await Assert.That(first.RequestBody).IsNotNull();
        await Assert.That(first.RequestBody).IsNotNullOrWhitespace();
    }
}
