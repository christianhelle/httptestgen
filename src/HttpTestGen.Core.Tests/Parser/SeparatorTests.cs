using HttpTestGen.Core;

namespace HttpTestGen.Core.Tests.Parser;

public class SeparatorTests
{
    [Test]
    [Arguments(
        """
        GET https://localhost/first
        ###
        GET https://localhost/second
        """)]
    [Arguments(
        """
        GET https://localhost/first

        ###

        GET https://localhost/second
        """)]
    public async Task Parse_Triple_Hash_Separator(string content)
    {
        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        await Assert.That(requests.Count).IsEqualTo(2);
        await Assert.That(requests[0].Endpoint).IsEqualTo("https://localhost/first");
        await Assert.That(requests[1].Endpoint).IsEqualTo("https://localhost/second");
    }

    [Test]
    public async Task Parse_Single_Hash_Separator()
    {
        var content = """
            GET https://localhost/first
            #
            GET https://localhost/second
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        await Assert.That(requests.Count).IsEqualTo(2);
    }

    [Test]
    public async Task Parse_Double_Hash_Separator()
    {
        var content = """
            GET https://localhost/first
            ##
            GET https://localhost/second
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        await Assert.That(requests.Count).IsEqualTo(2);
    }
}
