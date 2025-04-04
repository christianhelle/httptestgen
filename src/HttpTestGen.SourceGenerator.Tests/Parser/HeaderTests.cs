namespace HttpTestGen.SourceGenerator.Tests.Parser;

public class HeaderTests
{
    [Test]
    [Arguments(
        """
        GET https://localhost/ HTTP/1.1
        Accept: application/json
        Authorization: Bearer token
        x-custom-header: custom value
        """)]
    [Arguments(
        """
        #
        GET https://localhost/ HTTP/1.1
        Accept: application/json
        Authorization: Bearer token
        x-custom-header: custom value
        """)]
    [Arguments(
        """
        #
        GET https://localhost/ HTTP/1.1
        Accept: application/json
        Authorization: Bearer token
        x-custom-header: custom value
        #
        """)]
    [Arguments(
        """

        #

        GET https://localhost/ HTTP/1.1
        Accept: application/json
        Authorization: Bearer token
        x-custom-header: custom value


        """)]
    public async Task Parse_Headers_Request(string content)
    {
        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        var first = requests.Single();
        await Assert.That(first.Method).IsEqualTo("GET");
        await Assert.That(first.Endpoint).IsEqualTo("https://localhost/");
        await Assert.That(first.Headers["Accept"]).IsEqualTo("application/json");
        await Assert.That(first.Headers["Authorization"]).IsEqualTo("Bearer token");
        await Assert.That(first.Headers["x-custom-header"]).IsEqualTo("custom value");
    }
}