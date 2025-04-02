using System.Collections.Specialized;

namespace HttpTestGen.SourceGenerator;

public class HttpFileParser
{
    public IEnumerable<HttpFileRequest> Parse(string content)
    {
        var lines = content.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var requestLine = line.Split(' ');
            var method = requestLine[0];
            var endpoint = requestLine[1];
            var httpVersion = requestLine[2];

            yield return new HttpFileRequest
            {
                Method = method ?? "GET",
                Endpoint = endpoint.Trim(),
                HttpVersion = httpVersion.Trim(),
            };
        }
    }
}

public class HttpFileRequest
{
    public string Method { get; set; } = null!;
    public string Endpoint { get; set; } = null!;
    public string? HttpVersion { get; set; }
    public string? RequestBody { get; set; }
    public StringDictionary Headers { get; set; } = [];
}