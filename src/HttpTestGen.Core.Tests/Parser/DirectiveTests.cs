using HttpTestGen.Core;

namespace HttpTestGen.Core.Tests.Parser;

public class DirectiveTests
{
    [Test]
    [Arguments("# @name myRequest")]
    [Arguments("// @name myRequest")]
    public async Task Parse_Name_Directive(string directive)
    {
        var content = $"""
            {directive}
            GET https://localhost/
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        await Assert.That(requests.Single().Name).IsEqualTo("myRequest");
    }

    [Test]
    [Arguments("# @timeout 5s", 5000L)]
    [Arguments("// @timeout 5s", 5000L)]
    [Arguments("# @timeout 500ms", 500L)]
    [Arguments("# @timeout 2m", 120000L)]
    [Arguments("# @timeout 5000", 5000L)]
    public async Task Parse_Timeout_Directive(string directive, long expectedMs)
    {
        var content = $"""
            {directive}
            GET https://localhost/
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        await Assert.That(requests.Single().TimeoutMs).IsEqualTo(expectedMs);
    }

    [Test]
    [Arguments("# @connection-timeout 3s", 3000L)]
    [Arguments("// @connection-timeout 3s", 3000L)]
    [Arguments("# @connection-timeout 500ms", 500L)]
    public async Task Parse_Connection_Timeout_Directive(string directive, long expectedMs)
    {
        var content = $"""
            {directive}
            GET https://localhost/
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        await Assert.That(requests.Single().ConnectionTimeoutMs).IsEqualTo(expectedMs);
    }

    [Test]
    [Arguments("# @dependsOn loginRequest")]
    [Arguments("// @dependsOn loginRequest")]
    public async Task Parse_DependsOn_Directive(string directive)
    {
        var content = $"""
            {directive}
            GET https://localhost/
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        await Assert.That(requests.Single().DependsOn).IsEqualTo("loginRequest");
    }

    [Test]
    [Arguments("# @pre-delay 500", 500L)]
    [Arguments("// @pre-delay 1000", 1000L)]
    public async Task Parse_PreDelay_Directive(string directive, long expectedMs)
    {
        var content = $"""
            {directive}
            GET https://localhost/
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        await Assert.That(requests.Single().PreDelayMs).IsEqualTo(expectedMs);
    }

    [Test]
    [Arguments("# @post-delay 500", 500L)]
    [Arguments("// @post-delay 1000", 1000L)]
    public async Task Parse_PostDelay_Directive(string directive, long expectedMs)
    {
        var content = $"""
            {directive}
            GET https://localhost/
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        await Assert.That(requests.Single().PostDelayMs).IsEqualTo(expectedMs);
    }

    [Test]
    public async Task Parse_If_Directive()
    {
        var content = """
            # @if login.response.status 200
            GET https://localhost/protected
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        var req = requests.Single();
        await Assert.That(req.Conditions.Count).IsEqualTo(1);
        await Assert.That(req.Conditions[0].RequestName).IsEqualTo("login");
        await Assert.That(req.Conditions[0].Field).IsEqualTo("response.status");
        await Assert.That(req.Conditions[0].ExpectedValue).IsEqualTo("200");
        await Assert.That(req.Conditions[0].Negated).IsFalse();
    }

    [Test]
    public async Task Parse_IfNot_Directive()
    {
        var content = """
            // @if-not auth.response.status 401
            GET https://localhost/protected
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        var req = requests.Single();
        await Assert.That(req.Conditions.Count).IsEqualTo(1);
        await Assert.That(req.Conditions[0].RequestName).IsEqualTo("auth");
        await Assert.That(req.Conditions[0].Field).IsEqualTo("response.status");
        await Assert.That(req.Conditions[0].ExpectedValue).IsEqualTo("401");
        await Assert.That(req.Conditions[0].Negated).IsTrue();
    }

    [Test]
    public async Task Parse_Multiple_Conditions()
    {
        var content = """
            # @if login.response.status 200
            # @if-not auth.response.status 401
            GET https://localhost/protected
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        var req = requests.Single();
        await Assert.That(req.Conditions.Count).IsEqualTo(2);
    }

    [Test]
    public async Task Parse_Multiple_Directives_On_Single_Request()
    {
        var content = """
            # @name myRequest
            # @timeout 5s
            # @pre-delay 100
            # @post-delay 200
            # @dependsOn loginRequest
            GET https://localhost/
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        var req = requests.Single();
        await Assert.That(req.Name).IsEqualTo("myRequest");
        await Assert.That(req.TimeoutMs).IsEqualTo(5000L);
        await Assert.That(req.PreDelayMs).IsEqualTo(100L);
        await Assert.That(req.PostDelayMs).IsEqualTo(200L);
        await Assert.That(req.DependsOn).IsEqualTo("loginRequest");
    }
}
