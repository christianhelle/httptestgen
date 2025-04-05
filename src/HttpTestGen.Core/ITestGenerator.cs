namespace HttpTestGen.Core;

public interface ITestGenerator
{
    string Generate(
        string className,
        IList<HttpFileRequest> requests);
}