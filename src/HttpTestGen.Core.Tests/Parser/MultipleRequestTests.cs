using HttpTestGen.Core;

namespace HttpTestGen.Core.Tests.Parser;

public class MultipleRequestTests
{
    [Test]
    [Arguments(
        """
        GET https://localhost/ HTTP/1.1
        #

        GET https://localhost/foo HTTP/1.1
        #
        """
    )]
    [Arguments(
        """
        GET https://localhost/ HTTP/1.1
        ##

        GET https://localhost/foo HTTP/1.1
        ###
        """
    )]
    [Arguments(
        """
        GET https://localhost/ HTTP/1.1
        #


        GET https://localhost/foo HTTP/1.1
        #
        """
    )]
    [Arguments(
        """
        #
        GET https://localhost/

        #
        GET https://localhost/foo
        """
    )]
    public async Task Parse_Multiple_Request(string content)
    {
        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        var first = requests.First();
        await Assert.That(first.Method).IsEqualTo("GET");
        await Assert.That(first.Endpoint).IsEqualTo("https://localhost/");

        var second = requests.Skip(1).First();
        await Assert.That(second.Method).IsEqualTo("GET");
        await Assert.That(second.Endpoint).IsEqualTo("https://localhost/foo");
    }
}