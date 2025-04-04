namespace HttpTestGen.SourceGenerator.Tests.Parser;

public class AssertionsTests
{
    [Test]
    [Arguments(
        """
        GET https://localhost/notfound
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
}