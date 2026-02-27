using HttpTestGen.Core;

namespace HttpTestGen.Core.Tests.Parser;

public class FunctionSubstitutionTests
{
    [Test]
    public async Task Guid_Function_Produces_32_Hex_Chars()
    {
        var result = FunctionSubstitutor.SubstituteFunctions("{{guid()}}");
        await Assert.That(result.Length).IsEqualTo(32);
        await Assert.That(result.All(c => "0123456789abcdef".Contains(c))).IsTrue();
    }

    [Test]
    public async Task String_Function_Produces_20_Chars()
    {
        var result = FunctionSubstitutor.SubstituteFunctions("{{string()}}");
        await Assert.That(result.Length).IsEqualTo(20);
    }

    [Test]
    public async Task Number_Function_Produces_Number_0_To_100()
    {
        var result = FunctionSubstitutor.SubstituteFunctions("{{number()}}");
        var num = int.Parse(result);
        await Assert.That(num).IsGreaterThanOrEqualTo(0);
        await Assert.That(num).IsLessThanOrEqualTo(100);
    }

    [Test]
    public async Task Name_Function_Produces_Two_Words()
    {
        var result = FunctionSubstitutor.SubstituteFunctions("{{name()}}");
        var parts = result.Split(' ');
        await Assert.That(parts.Length).IsEqualTo(2);
        await Assert.That(parts[0].Length).IsGreaterThan(0);
        await Assert.That(parts[1].Length).IsGreaterThan(0);
    }

    [Test]
    public async Task FirstName_Function_Produces_NonEmpty()
    {
        var result = FunctionSubstitutor.SubstituteFunctions("{{first_name()}}");
        await Assert.That(result.Length).IsGreaterThan(0);
    }

    [Test]
    public async Task LastName_Function_Produces_NonEmpty()
    {
        var result = FunctionSubstitutor.SubstituteFunctions("{{last_name()}}");
        await Assert.That(result.Length).IsGreaterThan(0);
    }

    [Test]
    public async Task Email_Function_Produces_Valid_Format()
    {
        var result = FunctionSubstitutor.SubstituteFunctions("{{email()}}");
        await Assert.That(result).Contains("@");
        await Assert.That(result).Contains(".");
    }

    [Test]
    public async Task Address_Function_Produces_NonEmpty()
    {
        var result = FunctionSubstitutor.SubstituteFunctions("{{address()}}");
        await Assert.That(result.Length).IsGreaterThan(0);
    }

    [Test]
    public async Task JobTitle_Function_Produces_NonEmpty()
    {
        var result = FunctionSubstitutor.SubstituteFunctions("{{job_title()}}");
        await Assert.That(result.Length).IsGreaterThan(0);
    }

    [Test]
    public async Task GetDate_Function_Produces_Date_Format()
    {
        var result = FunctionSubstitutor.SubstituteFunctions("{{getdate()}}");
        await Assert.That(result).Matches(@"^\d{4}-\d{2}-\d{2}$");
    }

    [Test]
    public async Task GetTime_Function_Produces_Time_Format()
    {
        var result = FunctionSubstitutor.SubstituteFunctions("{{gettime()}}");
        await Assert.That(result).Matches(@"^\d{2}:\d{2}:\d{2}$");
    }

    [Test]
    public async Task GetDateTime_Function_Produces_DateTime_Format()
    {
        var result = FunctionSubstitutor.SubstituteFunctions("{{getdatetime()}}");
        await Assert.That(result).Matches(@"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}$");
    }

    [Test]
    public async Task GetUtcDateTime_Function_Produces_DateTime_Format()
    {
        var result = FunctionSubstitutor.SubstituteFunctions("{{getutcdatetime()}}");
        await Assert.That(result).Matches(@"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}$");
    }

    [Test]
    public async Task Base64Encode_Function()
    {
        var result = FunctionSubstitutor.SubstituteFunctions("{{base64_encode('hello')}}");
        await Assert.That(result).IsEqualTo("aGVsbG8=");
    }

    [Test]
    public async Task Upper_Function()
    {
        var result = FunctionSubstitutor.SubstituteFunctions("{{upper('hello world')}}");
        await Assert.That(result).IsEqualTo("HELLO WORLD");
    }

    [Test]
    public async Task Lower_Function()
    {
        var result = FunctionSubstitutor.SubstituteFunctions("{{lower('HELLO WORLD')}}");
        await Assert.That(result).IsEqualTo("hello world");
    }

    [Test]
    public async Task LoremIpsum_Function_With_Word_Count()
    {
        var result = FunctionSubstitutor.SubstituteFunctions("{{lorem_ipsum(5)}}");
        var words = result.Split(' ');
        await Assert.That(words.Length).IsEqualTo(5);
        await Assert.That(result).StartsWith("lorem");
    }

    [Test]
    public async Task Unknown_Function_Preserved()
    {
        var result = FunctionSubstitutor.SubstituteFunctions("{{unknown_func()}}");
        await Assert.That(result).IsEqualTo("{{unknown_func()}}");
    }

    [Test]
    public async Task Functions_In_Http_Content()
    {
        var content = """
            @id = {{guid()}}
            POST https://localhost/api
            Content-Type: application/json
            """;

        var sut = new HttpFileParser();
        var requests = sut.Parse(content).ToList();

        var req = requests.Single();
        await Assert.That(req.Endpoint).IsEqualTo("https://localhost/api");
    }

    [Test]
    public async Task Case_Insensitive_Functions()
    {
        var result = FunctionSubstitutor.SubstituteFunctions("{{GUID()}}");
        await Assert.That(result.Length).IsEqualTo(32);

        var result2 = FunctionSubstitutor.SubstituteFunctions("{{Upper('test')}}");
        await Assert.That(result2).IsEqualTo("TEST");
    }
}
