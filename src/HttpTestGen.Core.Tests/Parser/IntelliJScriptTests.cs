using HttpTestGen.Core;

namespace HttpTestGen.Core.Tests.Parser;

public class IntelliJScriptTests
{
    [Test]
    public async Task Parse_Ignores_IntelliJ_Script_Block()
    {
        var content = """
            GET https://localhost/
            > {%
                client.test("Test", function() {
                    client.assert(response.status === 200);
                });
            %}
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        await Assert.That(requests.Count).IsEqualTo(1);
        await Assert.That(requests.Single().Endpoint).IsEqualTo("https://localhost/");
    }

    [Test]
    public async Task Parse_IntelliJ_Script_Between_Requests()
    {
        var content = """
            GET https://localhost/first
            > {%
                client.test("Test", function() {});
            %}
            ###
            GET https://localhost/second
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        await Assert.That(requests.Count).IsEqualTo(2);
        await Assert.That(requests[0].Endpoint).IsEqualTo("https://localhost/first");
        await Assert.That(requests[1].Endpoint).IsEqualTo("https://localhost/second");
    }
}
