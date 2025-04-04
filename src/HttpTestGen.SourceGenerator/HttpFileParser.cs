using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;

namespace HttpTestGen.SourceGenerator;

public class HttpFileParser
{
    private static readonly Regex RegexUrl = new(
        @"^((?<method>get|post|patch|put|delete|head|options|trace))\s*(?<url>[^\s]+)\s*(?<version>HTTP/.*)?",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex RegexHeader = new(
        @"^(?<name>[^\s]+)?([\s]+)?(?<operator>:)(?<value>.+)",
        RegexOptions.Compiled);

    public IEnumerable<HttpFileRequest> Parse(string content)
    {
        HttpFileRequest? httpFileRequest = null;
        StringBuilder body = new();

        var lines = content.Split('\n');
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var match = RegexUrl.Match(line);
            if (match.Success)
            {
                if (httpFileRequest is not null)
                {
                    yield return httpFileRequest;
                }
                
                httpFileRequest = new()
                {
                    Method = match.Groups["method"].Value.Trim().ToUpperInvariant(),
                    Endpoint = match.Groups["url"].Value.Trim(),
                };
                continue;
            }

            match = RegexHeader.Match(line);
            if (match.Success && httpFileRequest is not null)
            {
                httpFileRequest!.Headers.Add(
                    match.Groups["name"].Value.Trim(),
                    match.Groups["value"].Value.Trim());
                continue;
            }

            if (line.Contains("EXPECTED_RESPONSE_STATUS") && httpFileRequest is not null)
            {
                var value = line.Split(' ')[1].Trim();
                httpFileRequest!.Assertions.ExpectedStatusCode = int.Parse(value);
                continue;
            }

            if (line.Contains("EXPECTED_RESPONSE_HEADER") && httpFileRequest is not null)
            {
                var header = line.Substring("EXPECTED_RESPONSE_HEADER".Length).Trim();
                var headerParts = header.Split(':');
                if (headerParts.Length != 2)
                {
                    continue;
                }

                var headerName = headerParts[0].Trim();
                var headerValue = headerParts[1].Trim();
                httpFileRequest!.Assertions.ExpectedHeaders.Add(headerName, headerValue);
                continue;
            }

            for (var j = i + 1; j < lines.Length; j++)
            {
                line = lines[j].Trim();
                if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line))
                    continue;
                body.AppendLine(line);
                if (RegexUrl.Match(line).Success)
                    break;
            }

            if (httpFileRequest == null) continue;
            if (body.Length > 0) 
                httpFileRequest.RequestBody = body.ToString();
            yield return httpFileRequest;
            httpFileRequest = null;
            body.Clear();
        }

        if (httpFileRequest != null)
            yield return httpFileRequest;
    }
}