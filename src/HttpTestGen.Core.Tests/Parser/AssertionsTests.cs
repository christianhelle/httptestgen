using HttpTestGen.Core;

namespace HttpTestGen.Core.Tests.Parser;

public class AssertionsTests
{
    [Test]
    [Arguments(
        """
        GET https://localhost/notfound
        EXPECTED_RESPONSE_STATUS 404
        """)]
    [Arguments(
        """
        GET https://localhost/notfound

        EXPECTED_RESPONSE_STATUS 404
        """)]
    [Arguments(
        """
        GET https://localhost/notfound
        Accept: application/json
        EXPECTED_RESPONSE_STATUS 404
        """)]
    [Arguments(
        """
        GET https://localhost/notfound
        Accept: application/json

        EXPECTED_RESPONSE_STATUS 404
        """)]
    public async Task Parse_Status_Assertions(string content)
    {
        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        var first = requests.Single();

        await Assert
            .That(first.Assertions.ExpectedStatusCode)
            .IsEqualTo(404);

        await Assert
            .That(first.Assertions.ExpectedHeaders)
            .IsEmpty();
    }

    [Test]
    [Arguments(
        """
        GET https://localhost/notfound
        Accept: application/json
        EXPECTED_RESPONSE_HEADER content-type: application/json
        EXPECTED_RESPONSE_HEADER x-custom-header: custom value
        """)]
    [Arguments(
        """
        GET https://localhost/notfound
        Accept: application/json

        EXPECTED_RESPONSE_HEADER content-type: application/json
        EXPECTED_RESPONSE_HEADER x-custom-header: custom value
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

    [Test]
    public async Task Parse_Status_Assertions_Malformed_Should_Not_Throw()
    {
        var sut = new HttpFileParser();
        var content = """
            GET https://localhost/notfound
            EXPECTED_RESPONSE_STATUS
            """;
        var requests = sut.Parse(content).ToList();

        var first = requests.Single();
        // Should use default status code (200) when malformed
        await Assert
            .That(first.Assertions.ExpectedStatusCode)
            .IsEqualTo(200);
    }

    [Test]
    public async Task Parse_Status_Assertions_Invalid_Value_Should_Not_Throw()
    {
        var sut = new HttpFileParser();
        var content = """
            GET https://localhost/notfound
            EXPECTED_RESPONSE_STATUS invalid
            """;
        var requests = sut.Parse(content).ToList();

        var first = requests.Single();
        // Should use default status code (200) when invalid
        await Assert
            .That(first.Assertions.ExpectedStatusCode)
            .IsEqualTo(200);
    }

    [Test]
    public async Task Parse_Header_Assertions_With_Url_Value()
    {
        var sut = new HttpFileParser();
        var content = """
            GET https://localhost/redirect
            EXPECTED_RESPONSE_HEADER Location: https://example.com/redirect
            """;
        var requests = sut.Parse(content).ToList();

        var first = requests.Single();
        await Assert
            .That(first.Assertions.ExpectedHeaders.ContainsKey("Location"))
            .IsTrue();
        await Assert
            .That(first.Assertions.ExpectedHeaders["Location"])
            .IsEqualTo("https://example.com/redirect");
    }

    [Test]
    public async Task Parse_Header_Assertions_Malformed_Should_Skip()
    {
        var sut = new HttpFileParser();
        var content = """
            GET https://localhost/notfound
            EXPECTED_RESPONSE_HEADER
            EXPECTED_RESPONSE_HEADER valid-header: value
            """;
        var requests = sut.Parse(content).ToList();

        var first = requests.Single();
        // Should only have the valid header
        await Assert
            .That(first.Assertions.ExpectedHeaders.Count)
            .IsEqualTo(1);
        await Assert
            .That(first.Assertions.ExpectedHeaders.ContainsKey("valid-header"))
            .IsTrue();
    }
}