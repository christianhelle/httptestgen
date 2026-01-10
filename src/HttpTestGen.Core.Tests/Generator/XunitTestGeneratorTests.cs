using HttpTestGen.Core;

namespace HttpTestGen.Core.Tests.Generator;

public class XunitTestGeneratorTests
{
    [Test]
    public async Task Generate_Get_Method_Uses_GetAsync()
    {
        var sut = new XunitTestGenerator();
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
    public async Task Generate_Post_Method_Uses_PostAsync()
    {
        var sut = new XunitTestGenerator();
        var request = new HttpFileRequest 
        { 
            Method = "POST", 
            Endpoint = "https://localhost/test" 
        };
        
        var code = sut.Generate("Test", new List<HttpFileRequest> { request });
        
        await Assert.That(code.Contains("PostAsync")).IsTrue();
        await Assert.That(code.Contains("SendAsync")).IsFalse();
    }

    [Test]
    public async Task Generate_Put_Method_Uses_PutAsync()
    {
        var sut = new XunitTestGenerator();
        var request = new HttpFileRequest 
        { 
            Method = "PUT", 
            Endpoint = "https://localhost/test" 
        };
        
        var code = sut.Generate("Test", new List<HttpFileRequest> { request });
        
        await Assert.That(code.Contains("PutAsync")).IsTrue();
        await Assert.That(code.Contains("SendAsync")).IsFalse();
    }

    [Test]
    public async Task Generate_Delete_Method_Uses_DeleteAsync()
    {
        var sut = new XunitTestGenerator();
        var request = new HttpFileRequest 
        { 
            Method = "DELETE", 
            Endpoint = "https://localhost/test" 
        };
        
        var code = sut.Generate("Test", new List<HttpFileRequest> { request });
        
        await Assert.That(code.Contains("DeleteAsync")).IsTrue();
        await Assert.That(code.Contains("SendAsync")).IsFalse();
    }

    [Test]
    public async Task Generate_Patch_Method_Uses_SendAsync_With_HttpMethod_Patch()
    {
        var sut = new XunitTestGenerator();
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
        var sut = new XunitTestGenerator();
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
        var sut = new XunitTestGenerator();
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
        var sut = new XunitTestGenerator();
        var request = new HttpFileRequest 
        { 
            Method = "TRACE", 
            Endpoint = "https://localhost/test" 
        };
        
        var code = sut.Generate("Test", new List<HttpFileRequest> { request });
        
        await Assert.That(code.Contains("SendAsync")).IsTrue();
        await Assert.That(code.Contains("HttpMethod.Trace")).IsTrue();
    }

    [Test]
    public async Task Generate_Patch_Method_Name_Uses_Original_Method()
    {
        var sut = new XunitTestGenerator();
        var request = new HttpFileRequest 
        { 
            Method = "PATCH", 
            Endpoint = "https://localhost/test" 
        };
        
        var code = sut.Generate("Test", new List<HttpFileRequest> { request });
        
        // Test method name should use the original HTTP method name
        await Assert.That(code.Contains("patch_localhost_0")).IsTrue();
    }
}
