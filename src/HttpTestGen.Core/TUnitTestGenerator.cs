using System.Text;

namespace HttpTestGen.Core;

public class TUnitTestGenerator : TestGenerator
{
    public override string Generate(
        string className,
        IList<HttpFileRequest> requests)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"public class {className}Tests");
        builder.AppendLine("{");
        for (var i = 0; i < requests.Count; i++)
        {
            GenerateMethod(requests[i], builder, i);
        }
        builder.AppendLine("}");
        return builder.ToString();
    }

    private static void GenerateMethod(
        HttpFileRequest request,
        StringBuilder builder,
        int index)
    {
        var requestName = new Uri(request.Endpoint).Host.Replace(".", "_").ToLowerInvariant();
        builder.AppendLine("    [Test]");
        builder.AppendLine($"    public async Task {request.Method.ToLowerInvariant()}_{requestName}_{index}()");
        builder.AppendLine("    {");
        builder.AppendLine("        var sut = new System.Net.Http.HttpClient();");
        
        if (NeedsSendAsync(request))
        {
            var httpMethodName = GetHttpMethodName(request);
            builder.AppendLine($"        var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.{httpMethodName}, \"{request.Endpoint}\");");
            builder.AppendLine("        var response = await sut.SendAsync(request);");
        }
        else
        {
            var httpMethod = GetMethod(request);
            builder.AppendLine($"        var response = await sut.{httpMethod}Async(\"{request.Endpoint}\");");
        }
        
        builder.AppendLine(
            request.Assertions.ExpectedStatusCode != 200
                ? $"        await Assert.That((int)response.StatusCode).IsEqualTo({request.Assertions.ExpectedStatusCode});"
                : "        await Assert.That(response.IsSuccessStatusCode).IsTrue();");
        foreach (var kvp in request.Assertions.ExpectedHeaders)
        {
            var key = kvp.Key;
            var value = kvp.Value;
            builder.AppendLine(
                $"        await Assert.That(response.Headers.GetValues(\"{key}\").Contains(\"{value}\")).IsTrue();");
        }

        builder.AppendLine("    }");
    }
}