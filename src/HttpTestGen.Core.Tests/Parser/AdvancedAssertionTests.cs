using HttpTestGen.Core;

namespace HttpTestGen.Core.Tests.Parser;

public class AdvancedAssertionTests
{
    [Test]
    public async Task Parse_Expected_Response_Body()
    {
        var content = """
            GET https://localhost/
            EXPECTED_RESPONSE_BODY "hello world"
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        await Assert.That(requests.Single().Assertions.ExpectedBody).IsEqualTo("hello world");
    }

    [Test]
    public async Task Parse_Expected_Response_Body_Without_Quotes()
    {
        var content = """
            GET https://localhost/
            EXPECTED_RESPONSE_BODY hello
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        await Assert.That(requests.Single().Assertions.ExpectedBody).IsEqualTo("hello");
    }

    [Test]
    public async Task Parse_Assertions_With_Greater_Than_Prefix()
    {
        var content = """
            GET https://localhost/
            > EXPECTED_RESPONSE_STATUS 404
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        await Assert.That(requests.Single().Assertions.ExpectedStatusCode).IsEqualTo(404);
    }

    [Test]
    public async Task Parse_Body_Assertion_With_Greater_Than_Prefix()
    {
        var content = """
            GET https://localhost/
            > EXPECTED_RESPONSE_BODY "expected text"
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        await Assert.That(requests.Single().Assertions.ExpectedBody).IsEqualTo("expected text");
    }

    [Test]
    public async Task Parse_Header_Assertion_With_Greater_Than_Prefix()
    {
        var content = """
            GET https://localhost/
            > EXPECTED_RESPONSE_HEADER content-type: application/json
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        await Assert.That(requests.Single().Assertions.ExpectedHeaders["content-type"]).IsEqualTo("application/json");
    }

    [Test]
    public async Task Parse_Expected_Response_Headers_Plural()
    {
        var content = """
            GET https://localhost/
            > EXPECTED_RESPONSE_HEADERS "content-type: application/json"
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        await Assert.That(requests.Single().Assertions.ExpectedHeaders["content-type"]).IsEqualTo("application/json");
    }
}
