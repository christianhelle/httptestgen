namespace HttpTestGen.Core;

public abstract class TestGenerator : ITestGenerator
{
    public abstract string Generate(
        string className,
        IList<HttpFileRequest> requests);

    protected static string GetMethod(HttpFileRequest request)
    {
        return request.Method.ToLowerInvariant() switch
        {
            "get" => "Get",
            "post" => "Post",
            "put" => "Put",
            "delete" => "Delete",
            "patch" => "Patch",
            "head" => "Send",
            "options" => "Send",
            "trace" => "Send",
            "connect" => "Send",
            _ => request.Method,
        };
    }

    protected static string GetTestMethodName(HttpFileRequest request, int index)
    {
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            return SanitizeIdentifier(request.Name);
        }

        var method = request.Method.ToLowerInvariant();
        string host;
        if (Uri.TryCreate(request.Endpoint, UriKind.Absolute, out var uri))
        {
            host = uri.Host.Replace(".", "_").Replace("-", "_").ToLowerInvariant();
        }
        else
        {
            host = request.Endpoint.Replace("/", "_").Replace(".", "_").Replace("-", "_")
                .Trim('_').ToLowerInvariant();
        }

        return $"{method}_{host}_{index}";
    }

    protected static bool NeedsSendAsync(HttpFileRequest request)
    {
        var m = request.Method.ToUpperInvariant();
        return m == "HEAD" || m == "OPTIONS" || m == "TRACE" || m == "CONNECT" || m == "PATCH";
    }

    protected static string GetHttpMethodEnum(HttpFileRequest request)
    {
        return request.Method.ToUpperInvariant() switch
        {
            "HEAD" => "System.Net.Http.HttpMethod.Head",
            "OPTIONS" => "System.Net.Http.HttpMethod.Options",
            "TRACE" => "System.Net.Http.HttpMethod.Trace",
            "PATCH" => "System.Net.Http.HttpMethod.Patch",
            "CONNECT" => "new System.Net.Http.HttpMethod(\"CONNECT\")",
            _ => $"System.Net.Http.HttpMethod.{request.Method[0]}{request.Method.Substring(1).ToLowerInvariant()}",
        };
    }

    private static string SanitizeIdentifier(string name)
    {
        var sb = new System.Text.StringBuilder();
        foreach (var c in name)
        {
            if (char.IsLetterOrDigit(c) || c == '_')
                sb.Append(c);
            else
                sb.Append('_');
        }
        var result = sb.ToString();
        if (result.Length > 0 && char.IsDigit(result[0]))
            result = "_" + result;
        return result;
    }
}