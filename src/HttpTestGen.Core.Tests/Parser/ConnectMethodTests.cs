using HttpTestGen.Core;

namespace HttpTestGen.Core.Tests.Parser;

public class ConnectMethodTests
{
    [Test]
    public async Task Parse_Connect_Method()
    {
        var content = """
            CONNECT https://localhost:443
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        await Assert.That(requests.Single().Method).IsEqualTo("CONNECT");
        await Assert.That(requests.Single().Endpoint).IsEqualTo("https://localhost:443");
    }
}
