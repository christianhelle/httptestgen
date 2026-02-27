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
        var method = GetMethod(request);
        var testName = GetTestMethodName(request, index);
        builder.AppendLine("    [Test]");
        builder.AppendLine($"    public async Task {testName}()");
        builder.AppendLine("    {");
        builder.AppendLine("        var sut = new System.Net.Http.HttpClient();");

        if (request.TimeoutMs.HasValue)
        {
            builder.AppendLine($"        sut.Timeout = System.TimeSpan.FromMilliseconds({request.TimeoutMs.Value});");
        }

        if (request.PreDelayMs.HasValue)
        {
            builder.AppendLine($"        await System.Threading.Tasks.Task.Delay({request.PreDelayMs.Value});");
        }

        if (NeedsSendAsync(request))
        {
            builder.AppendLine($"        var request = new System.Net.Http.HttpRequestMessage({GetHttpMethodEnum(request)}, \"{request.Endpoint}\");");
            if (!string.IsNullOrWhiteSpace(request.RequestBody))
            {
                var escapedBody = EscapeString(request.RequestBody);
                builder.AppendLine($"        request.Content = new System.Net.Http.StringContent(\"{escapedBody}\", System.Text.Encoding.UTF8, \"{GetContentType(request)}\");");
            }
            builder.AppendLine("        var response = await sut.SendAsync(request);");
        }
        else if (!string.IsNullOrWhiteSpace(request.RequestBody))
        {
            var escapedBody = EscapeString(request.RequestBody);
            var contentType = GetContentType(request);
            builder.AppendLine($"        var content = new System.Net.Http.StringContent(\"{escapedBody}\", System.Text.Encoding.UTF8, \"{contentType}\");");
            builder.AppendLine($"        var response = await sut.{method}Async(\"{request.Endpoint}\", content);");
        }
        else if (method == "Post" || method == "Put" || method == "Patch")
        {
            builder.AppendLine($"        var response = await sut.{method}Async(\"{request.Endpoint}\", null);");
        }
        else
        {
            builder.AppendLine($"        var response = await sut.{method}Async(\"{request.Endpoint}\");");
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

        if (!string.IsNullOrWhiteSpace(request.Assertions.ExpectedBody))
        {
            var escapedExpected = EscapeString(request.Assertions.ExpectedBody);
            builder.AppendLine("        var responseBody = await response.Content.ReadAsStringAsync();");
            builder.AppendLine($"        await Assert.That(responseBody).Contains(\"{escapedExpected}\");");
        }

        if (request.PostDelayMs.HasValue)
        {
            builder.AppendLine($"        await System.Threading.Tasks.Task.Delay({request.PostDelayMs.Value});");
        }

        builder.AppendLine("    }");
    }

    private static string GetContentType(HttpFileRequest request)
    {
        if (request.Headers["Content-Type"] != null)
            return request.Headers["Content-Type"];
        return "application/json";
    }

    private static string EscapeString(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n")
            .Replace("\t", "\\t");
    }
}