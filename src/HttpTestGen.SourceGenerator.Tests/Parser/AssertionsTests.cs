namespace HttpTestGen.SourceGenerator.Tests.Parser;

public class AssertionsTests
{
    [Test]
    [Arguments(
        """
        GET https://localhost/notfound
        EXPECTED_STATUS: 404
        """)]
    [Arguments(
        """
        GET https://localhost/notfound

        EXPECTED_STATUS: 404
        """)]
    [Arguments(
        """
        GET https://localhost/notfound
        Accept: application/json
        EXPECTED_STATUS: 404
        """)]
    [Arguments(
        """
        GET https://localhost/notfound
        Accept: application/json

        EXPECTED_STATUS: 404
        """)]
    public async Task Parse_Assertions(string content)
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
        EXPECTED_STATUS: 404
        """)]
    [Arguments(
        """
        GET https://localhost/notfound
        Accept: application/json

        EXPECTED_STATUS: 404
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
            .That(first.Assertions.ExpectedStatusCode)
            .IsEqualTo(404);
    }
}