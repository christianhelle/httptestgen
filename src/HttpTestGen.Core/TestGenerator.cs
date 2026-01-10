namespace HttpTestGen.Core;

public abstract class TestGenerator : ITestGenerator
{
    public abstract string Generate(
        string className,
        IList<HttpFileRequest> requests);

    protected static bool NeedsSendAsync(HttpFileRequest request)
    {
        return request.Method.ToUpperInvariant() switch
        {
            "GET" or "POST" or "PUT" or "DELETE" => false,
            _ => true
        };
    }

    protected static string GetHttpMethodName(HttpFileRequest request)
    {
        // Returns the proper casing for System.Net.Http.HttpMethod.{Name}
        return request.Method.ToUpperInvariant() switch
        {
            "GET" => "Get",
            "POST" => "Post",
            "PUT" => "Put",
            "DELETE" => "Delete",
            "PATCH" => "Patch",
            "HEAD" => "Head",
            "OPTIONS" => "Options",
            "TRACE" => "Trace",
            "CONNECT" => "Connect",
            _ => request.Method, // Fallback to original method name for custom methods
        };
    }

    protected static string GetMethod(HttpFileRequest request)
    {
        return request.Method.ToLowerInvariant() switch
        {
            "get" => "Get",
            "post" => "Post",
            "put" => "Put",
            "delete" => "Delete",
            _ => "Send",
        };
    }
}