using System.Text;

namespace HttpTestGen.Core;

public class XunitTestGenerator : TestGenerator
{
    public override string Generate(
        string className,
        IList<HttpFileRequest> requests)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"public class {className}HttpTests");
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
        var method = GetMethod(request);
        var requestName = new Uri(request.Endpoint).Host.Replace(".", "_").ToLowerInvariant();
        builder.AppendLine("    [Xunit.Fact]");
        builder.AppendLine($"    public async Task {method.ToLowerInvariant()}_{requestName}_{index}()");
        builder.AppendLine("    {");
        builder.AppendLine("        var sut = new System.Net.Http.HttpClient();");
        builder.AppendLine($"        var response = await sut.{method}Async(\"{request.Endpoint}\");");
        builder.AppendLine(
            request.Assertions.ExpectedStatusCode != 200
                ? $"        Xunit.Assert.Equal({request.Assertions.ExpectedStatusCode}, (int)response.StatusCode);"
                : "        Xunit.Assert.True(response.IsSuccessStatusCode);");
        foreach (var kvp in request.Assertions.ExpectedHeaders)
        {
            builder.AppendLine(
                $"        Xunit.Assert.True(response.Headers.GetValues(\"{kvp.Key}\").Contains(\"{kvp.Value}\"));");
        }

        builder.AppendLine("    }");
    }
}