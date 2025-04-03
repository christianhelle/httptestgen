using Xunit;

namespace HttpTestGen.SourceGenerator.Tests.Parser;

public class MultipleRequestTests
{
    [Theory]
    [InlineData(
        """
        GET https://localhost/ HTTP/1.1
        #

        GET https://localhost/foo HTTP/1.1
        #
        """
    )]
    [InlineData(
        """
        GET https://localhost/ HTTP/1.1
        ##

        GET https://localhost/foo HTTP/1.1
        ###
        """
    )]
    [InlineData(
        """
        GET https://localhost/ HTTP/1.1
        #


        GET https://localhost/foo HTTP/1.1
        #
        """
    )]
    [InlineData(
        """
        #
        GET https://localhost/

        #
        GET https://localhost/foo
        """
    )]
    public void Parse_Multiple_Request(string content)
    {
        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        var first = requests.First();
        Assert.Equal("GET", first.Method);
        Assert.Equal("https://localhost/", first.Endpoint);

        var second = requests.Skip(1).First();
        Assert.Equal("GET", second.Method);
        Assert.Equal("https://localhost/foo", second.Endpoint);
    }
}
