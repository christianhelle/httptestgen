using HttpTestGen.Core;

namespace HttpTestGen.Core.Tests.Parser;

public class VariableTests
{
    [Test]
    public async Task Parse_Variable_Declaration_And_Substitution()
    {
        var content = """
            @host = localhost
            GET https://{{host}}/api
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        var first = requests.Single();
        await Assert.That(first.Endpoint).IsEqualTo("https://localhost/api");
    }

    [Test]
    public async Task Parse_Multiple_Variables()
    {
        var content = """
            @host = localhost
            @port = 8080
            GET https://{{host}}:{{port}}/api
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        var first = requests.Single();
        await Assert.That(first.Endpoint).IsEqualTo("https://localhost:8080/api");
    }

    [Test]
    public async Task Parse_Variable_In_Header()
    {
        var content = """
            @token = abc123
            GET https://localhost/
            Authorization: Bearer {{token}}
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        var first = requests.Single();
        await Assert.That(first.Headers["Authorization"]).IsEqualTo("Bearer abc123");
    }

    [Test]
    public async Task Parse_Variable_Chaining()
    {
        var content = """
            @host = example.com
            @baseUrl = https://{{host}}
            GET {{baseUrl}}/api
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        var first = requests.Single();
        await Assert.That(first.Endpoint).IsEqualTo("https://example.com/api");
    }

    [Test]
    public async Task Parse_Variable_Override()
    {
        var content = """
            @host = old.com
            @host = new.com
            GET https://{{host}}/
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        await Assert.That(requests.Single().Endpoint).IsEqualTo("https://new.com/");
    }
}
