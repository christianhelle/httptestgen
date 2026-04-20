

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
        await Assert.That(!string.IsNullOrWhiteSpace(first.RequestBody)).IsTrue();
    }

    [Test]
    public async Task Parse_Body_Should_Not_Include_Next_Request_Url()
    {
        var sut = new HttpFileParser();
        var content = """
            POST https://localhost/first HTTP/1.1
            Content-Type: application/json

            {
              "data": "test"
            }

            GET https://localhost/second HTTP/1.1
            """;
        var requests = sut.Parse(content).ToList();

        await Assert.That(requests.Count).IsEqualTo(2);

        var first = requests[0];
        await Assert.That(first.Method).IsEqualTo("POST");
        await Assert.That(first.Endpoint).IsEqualTo("https://localhost/first");
        await Assert.That(first.RequestBody).IsNotNull();
        await Assert.That(first.RequestBody!.Contains("localhost/second")).IsFalse();
        await Assert.That(first.RequestBody!.Contains("GET")).IsFalse();

        var second = requests[1];
        await Assert.That(second.Method).IsEqualTo("GET");
        await Assert.That(second.Endpoint).IsEqualTo("https://localhost/second");
    }
}
