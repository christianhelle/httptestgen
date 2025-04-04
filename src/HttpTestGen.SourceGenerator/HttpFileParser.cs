﻿using System.Collections.Specialized;
using System.Text;

namespace HttpTestGen.SourceGenerator;

public class HttpFileParser
{
    public IEnumerable<HttpFileRequest> Parse(string content)
    {
        var lines = content.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (line.StartsWith("#"))
            {
                continue;
            }

            var requestLine = line
                .TrimStart()
                .Split([" "], StringSplitOptions.RemoveEmptyEntries);
            if (requestLine.Length < 2)
            {
                continue;
            }

            var method = requestLine[0];
            var endpoint = requestLine[1];

            var headers = new StringDictionary();
            for (int j = i + 1; j < lines.Length; j++)
            {
                if (lines[j].StartsWith("#"))
                {
                    break;
                }

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

            StringBuilder body = new();
            for (int j = i + 1; j < lines.Length; j++)
            {
                if (lines[j].StartsWith("#"))
                {
                    break;
                }
                body.Append(lines[j]);
                i++;
            }

            yield return new HttpFileRequest
            {
                Method = method,
                Endpoint = endpoint.Trim(),
                Headers = headers,
                RequestBody = body.ToString().Trim(),
            };
        }
    }
}
