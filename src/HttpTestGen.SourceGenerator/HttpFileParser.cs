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
            var headers = new StringDictionary();
            for (int j = i; j < lines.Length; j++)
            {
                var split = lines[j].Split(':');
                if (split.Length == 2)
                {
                    headers[split[0].Trim()] = split[1].Trim();
                    i++;
                }
                else
                {
                    break;
                }
            }

            yield return new HttpFileRequest
            {
                Method = method ?? "GET",
                Endpoint = endpoint.Trim(),
                HttpVersion = httpVersion.Trim(),
                Headers = headers,
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