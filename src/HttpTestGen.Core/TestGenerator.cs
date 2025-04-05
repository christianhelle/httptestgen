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
            _ => request.Method,
        };
    }
}