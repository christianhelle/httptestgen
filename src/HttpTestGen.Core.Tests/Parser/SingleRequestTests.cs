using HttpTestGen.Core;

namespace HttpTestGen.Core.Tests.Parser;

public class SingleRequestTests
{
    [Test]
    [Arguments("GET")]
    [Arguments("POST")]
    [Arguments("PUT")]
    [Arguments("DELETE")]
    [Arguments(" GET")]
    [Arguments(" POST")]
    [Arguments(" PUT")]
    [Arguments(" DELETE")]
    [Arguments("  GET")]
    [Arguments("  POST")]
    [Arguments("  PUT")]
    [Arguments("  DELETE")]
    [Arguments("GET ")]
    [Arguments("POST ")]
    [Arguments("PUT ")]
    [Arguments("DELETE ")]
    [Arguments("GET  ")]
    [Arguments("POST  ")]
    [Arguments("PUT  ")]
    [Arguments("DELETE  ")]
    [Arguments(" GET ")]
    [Arguments(" POST ")]
    [Arguments(" PUT ")]
    [Arguments(" DELETE ")]
    [Arguments("  GET  ")]
    [Arguments("  POST  ")]
    [Arguments("  PUT  ")]
    [Arguments("  DELETE  ")]
    public async Task Parse_Verb(string verb)
    {
        var sut = new HttpFileParser();
        var requests = sut.Parse($"{verb} /");

        var result = requests.Single();
        await Assert
            .That(result.Method)
            .IsEqualTo(verb.Trim());
    }

    [Test]
    [Arguments("GET /")]
    [Arguments("GET  /")]
    [Arguments(" GET  /")]
    [Arguments(" GET / ")]
    [Arguments("GET / ")]
    [Arguments("POST /")]
    [Arguments(" POST  /")]
    [Arguments("POST  /")]
    [Arguments("POST / ")]
    [Arguments("PUT /")]
    [Arguments(" PUT  /")]
    [Arguments("PUT  /")]
    [Arguments("PUT / ")]
    [Arguments("DELETE /")]
    [Arguments(" DELETE  /")]
    [Arguments("DELETE  /")]
    [Arguments("DELETE / ")]
    public async Task Parse_Endpoint(string content)
    {
        var sut = new HttpFileParser();
        var requests = sut.Parse(content);

        var result = requests.Single();
        await Assert.That(result.Endpoint).IsEqualTo("/");
    }
}