using FluentAssertions;

namespace HttpTestGen.SourceGenerator.Tests.Parser;

public class AssertionsTests
{
    [Test]
    [Arguments(
        """
        GET https://localhost/notfound
        EXPECTED_RESPONSE_STATUS: 404
        """)]
    [Arguments(
        """
        GET https://localhost/notfound

        EXPECTED_RESPONSE_STATUS: 404
        """)]
    [Arguments(
        """
        GET https://localhost/notfound
        Accept: application/json
        EXPECTED_RESPONSE_STATUS: 404
        """)]
    [Arguments(
        """
        GET https://localhost/notfound
        Accept: application/json

        EXPECTED_RESPONSE_STATUS: 404
        """)]
    public async Task Parse_Status_Assertions(string content)
    {
        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        var first = requests.Single();

        await Assert
            .That(first.Assertions.ExpectedStatusCode)
            .IsEqualTo(404);
    }
    [Test]
    [Arguments(
        """
        GET https://localhost/notfound
        Accept: application/json
        EXPECTED_RESPONSE_HEADER: content-type: application/json
        EXPECTED_RESPONSE_HEADER: x-custom-header: custom value
        """)]
    [Arguments(
        """
        GET https://localhost/notfound
        Accept: application/json
        
        EXPECTED_RESPONSE_HEADER: content-type: application/json
        EXPECTED_RESPONSE_HEADER: x-custom-header: custom value
        """)]
    public async Task Parse_Header_Assertions(string content)
    {
        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        var first = requests.Single();

        await Assert
            .That(first.Method)
            .IsEqualTo("GET");
        await Assert
            .That(first.Endpoint)
            .IsEqualTo("https://localhost/notfound");
        await Assert
            .That(first.Headers.ContainsKey("Accept"))
            .IsTrue();
        await Assert
            .That(first.Headers["Accept"])
            .IsEqualTo("application/json");
        await Assert
            .That(first.Assertions.ExpectedHeaders.ContainsKey("x-custom-header"))
            .IsTrue();
        await Assert
            .That(first.Assertions.ExpectedHeaders["x-custom-header"])
            .IsEqualTo("custom value");
        await Assert
            .That(first.Assertions.ExpectedHeaders.ContainsKey("content-type"))
            .IsTrue();
        await Assert
            .That(first.Assertions.ExpectedHeaders["content-type"])
            .IsEqualTo("application/json");
    }
}