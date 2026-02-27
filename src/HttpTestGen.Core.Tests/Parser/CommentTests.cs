using HttpTestGen.Core;

namespace HttpTestGen.Core.Tests.Parser;

public class CommentTests
{
    [Test]
    [Arguments(
        """
        # This is a comment
        GET https://localhost/
        """)]
    [Arguments(
        """
        // This is a comment
        GET https://localhost/
        """)]
    [Arguments(
        """
        # comment 1
        // comment 2
        GET https://localhost/
        """)]
    public async Task Parse_Both_Comment_Styles(string content)
    {
        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        var first = requests.Single();
        await Assert.That(first.Method).IsEqualTo("GET");
        await Assert.That(first.Endpoint).IsEqualTo("https://localhost/");
    }

    [Test]
    public async Task Parse_Inline_Comments_Not_Breaking_Requests()
    {
        var content = """
            // comment before
            GET https://localhost/first
            // comment between
            ###
            // comment after separator
            GET https://localhost/second
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        await Assert.That(requests.Count).IsEqualTo(2);
        await Assert.That(requests[0].Endpoint).IsEqualTo("https://localhost/first");
        await Assert.That(requests[1].Endpoint).IsEqualTo("https://localhost/second");
    }
}
