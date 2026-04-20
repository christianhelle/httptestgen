using HttpTestGen.Core;

namespace HttpTestGen.Core.Tests.Generator;

public class TUnitTestGeneratorTests
{
    [Test]
    public async Task Generate_Get_Method_Uses_GetAsync()
    {
        var sut = new TUnitTestGenerator();
        var request = new HttpFileRequest 
        { 
            Method = "GET", 
            Endpoint = "https://localhost/test" 
        };
        
        var code = sut.Generate("Test", new List<HttpFileRequest> { request });
        
        await Assert.That(code.Contains("GetAsync")).IsTrue();
        await Assert.That(code.Contains("SendAsync")).IsFalse();
    }

    [Test]
    public async Task Generate_Patch_Method_Uses_SendAsync_With_HttpMethod_Patch()
    {
        var sut = new TUnitTestGenerator();
        var request = new HttpFileRequest 
        { 
            Method = "PATCH", 
            Endpoint = "https://localhost/test" 
        };
        
        var code = sut.Generate("Test", new List<HttpFileRequest> { request });
        
        await Assert.That(code.Contains("SendAsync")).IsTrue();
        await Assert.That(code.Contains("HttpMethod.Patch")).IsTrue();
    }

    [Test]
    public async Task Generate_Head_Method_Uses_SendAsync_With_HttpMethod_Head()
    {
        var sut = new TUnitTestGenerator();
        var request = new HttpFileRequest 
        { 
            Method = "HEAD", 
            Endpoint = "https://localhost/test" 
        };
        
        var code = sut.Generate("Test", new List<HttpFileRequest> { request });
        
        await Assert.That(code.Contains("SendAsync")).IsTrue();
        await Assert.That(code.Contains("HttpMethod.Head")).IsTrue();
    }

    [Test]
    public async Task Generate_Options_Method_Uses_SendAsync_With_HttpMethod_Options()
    {
        var sut = new TUnitTestGenerator();
        var request = new HttpFileRequest 
        { 
            Method = "OPTIONS", 
            Endpoint = "https://localhost/test" 
        };
        
        var code = sut.Generate("Test", new List<HttpFileRequest> { request });
        
        await Assert.That(code.Contains("SendAsync")).IsTrue();
        await Assert.That(code.Contains("HttpMethod.Options")).IsTrue();
    }

    [Test]
    public async Task Generate_Trace_Method_Uses_SendAsync_With_HttpMethod_Trace()
    {
        var sut = new TUnitTestGenerator();
        var request = new HttpFileRequest 
        { 
            Method = "TRACE", 
            Endpoint = "https://localhost/test" 
        };
        
        var code = sut.Generate("Test", new List<HttpFileRequest> { request });
        
        await Assert.That(code.Contains("SendAsync")).IsTrue();
        await Assert.That(code.Contains("HttpMethod.Trace")).IsTrue();
    }
}
