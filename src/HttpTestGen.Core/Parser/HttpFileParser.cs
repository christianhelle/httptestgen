using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace HttpTestGen.Core;

public class HttpFileParser
{
    private static readonly Regex RegexUrl = new Regex(
        @"^((?<method>get|post|patch|put|delete|head|options|trace|connect))\s+(?<url>[^\s]+)\s*(?<version>HTTP/.*)?",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex RegexHeader = new Regex(
        @"^(?<name>[^\s:]+)\s*:\s*(?<value>.+)",
        RegexOptions.Compiled);

    public IEnumerable<HttpFileRequest> Parse(string content)
    {
        var state = new ParserState();

        var lines = content.Split('\n');
        for (var i = 0; i < lines.Length; i++)
        {
            ParseLine(lines[i], state);
        }

        // Finalize last request
        if (state.CurrentRequest != null)
        {
            FinalizeRequest(state);
        }

        return state.Requests;
    }

    private static void ParseLine(string line, ParserState state)
    {
        var trimmed = line.Trim();

        // Handle IntelliJ script blocks: > {% ... %}
        if (trimmed.StartsWith("> {%"))
        {
            state.InIntelliJScript = true;
            return;
        }

        if (state.InIntelliJScript)
        {
            if (trimmed == "%}" || trimmed.EndsWith("%}"))
                state.InIntelliJScript = false;
            return;
        }

        // Handle empty lines
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            if (state.InBody)
                state.BodyContent.AppendLine();
            else if (state.CurrentRequest != null)
                state.InBody = true;
            return;
        }

        // Try directives (before comment handling since directives start with # or //)
        if (TryParseDirective(trimmed, state))
            return;

        // Handle ### request separator
        if (IsRequestSeparator(trimmed))
        {
            if (state.CurrentRequest != null)
                FinalizeRequest(state);
            return;
        }

        // Skip comment lines (after directive processing)
        if (trimmed.StartsWith("#") || trimmed.StartsWith("//"))
            return;

        // Try variable declarations (@name = value)
        if (TryParseVariable(trimmed, state))
            return;

        // Try assertions (EXPECTED_RESPONSE_* or > EXPECTED_RESPONSE_*)
        if (TryParseAssertion(trimmed, state))
            return;

        // Try HTTP request line
        var match = RegexUrl.Match(trimmed);
        if (match.Success)
        {
            if (state.CurrentRequest != null)
                FinalizeRequest(state);

            var url = VariableSubstitutor.SubstituteAll(
                match.Groups["url"].Value.Trim(), state.Variables);

            state.CurrentRequest = new HttpFileRequest
            {
                Name = state.PendingName,
                Method = match.Groups["method"].Value.Trim().ToUpperInvariant(),
                Endpoint = url,
                TimeoutMs = state.PendingTimeout,
                ConnectionTimeoutMs = state.PendingConnectionTimeout,
                DependsOn = state.PendingDependsOn,
                PreDelayMs = state.PendingPreDelay,
                PostDelayMs = state.PendingPostDelay,
            };
            if (state.PendingConditions.Count > 0)
            {
                state.CurrentRequest.Conditions.AddRange(state.PendingConditions);
            }
            state.ClearPending();
            state.InBody = false;
            return;
        }

        // Try header line (must have colon and not be in body)
        if (!state.InBody && state.CurrentRequest != null)
        {
            var headerMatch = RegexHeader.Match(trimmed);
            if (headerMatch.Success)
            {
                var name = VariableSubstitutor.SubstituteAll(
                    headerMatch.Groups["name"].Value.Trim(), state.Variables);
                var value = VariableSubstitutor.SubstituteAll(
                    headerMatch.Groups["value"].Value.Trim(), state.Variables);
                state.CurrentRequest.Headers.Add(name, value);
                return;
            }
        }

        // Must be body content
        if (state.CurrentRequest != null)
        {
            state.InBody = true;
            state.BodyContent.AppendLine(VariableSubstitutor.SubstituteAll(line, state.Variables));
        }
    }

    private static bool IsRequestSeparator(string trimmed)
    {
        // ### is the standard separator; also treat # and ## as separators for compat
        if (trimmed.Length == 0) return false;
        for (int i = 0; i < trimmed.Length; i++)
        {
            if (trimmed[i] != '#') return false;
        }
        return true;
    }

    private static bool TryParseDirective(string trimmed, ParserState state)
    {
        // Extract directive content from # or // prefix
        string? directiveContent = null;
        if (trimmed.StartsWith("# @"))
            directiveContent = trimmed.Substring(2).Trim();
        else if (trimmed.StartsWith("// @"))
            directiveContent = trimmed.Substring(3).Trim();

        if (directiveContent == null)
            return false;

        // @name
        if (directiveContent.StartsWith("@name "))
        {
            state.PendingName = directiveContent.Substring(6).Trim();
            return true;
        }

        // @connection-timeout (must check before @timeout since it's longer prefix)
        if (directiveContent.StartsWith("@connection-timeout "))
        {
            var value = directiveContent.Substring(20).Trim();
            state.PendingConnectionTimeout = TimeoutParser.Parse(value);
            return true;
        }

        // @timeout
        if (directiveContent.StartsWith("@timeout "))
        {
            var value = directiveContent.Substring(9).Trim();
            state.PendingTimeout = TimeoutParser.Parse(value);
            return true;
        }

        // @dependsOn
        if (directiveContent.StartsWith("@dependsOn "))
        {
            state.PendingDependsOn = directiveContent.Substring(11).Trim();
            return true;
        }

        // @if-not (check before @if)
        if (directiveContent.StartsWith("@if-not "))
        {
            var condition = ParseCondition(directiveContent.Substring(8).Trim(), true);
            if (condition != null)
                state.PendingConditions.Add(condition);
            return true;
        }

        // @if
        if (directiveContent.StartsWith("@if "))
        {
            var condition = ParseCondition(directiveContent.Substring(4).Trim(), false);
            if (condition != null)
                state.PendingConditions.Add(condition);
            return true;
        }

        // @pre-delay
        if (directiveContent.StartsWith("@pre-delay "))
        {
            var value = directiveContent.Substring(11).Trim();
            if (long.TryParse(value, out var delay))
                state.PendingPreDelay = delay;
            return true;
        }

        // @post-delay
        if (directiveContent.StartsWith("@post-delay "))
        {
            var value = directiveContent.Substring(12).Trim();
            if (long.TryParse(value, out var delay))
                state.PendingPostDelay = delay;
            return true;
        }

        return false;
    }

    private static HttpFileCondition? ParseCondition(string value, bool negated)
    {
        // Format: requestName.response.status 200
        // Format: requestName.response.body.$.token "value"
        var parts = value.Split(new[] { ' ' }, 2);
        if (parts.Length < 2)
            return null;

        var field = parts[0].Trim();
        var expectedValue = StripQuotes(parts[1].Trim());

        // Extract request name from field (everything before first '.')
        var dotIndex = field.IndexOf('.');
        if (dotIndex < 0)
            return null;

        var requestName = field.Substring(0, dotIndex);
        var fieldPath = field.Substring(dotIndex + 1);

        return new HttpFileCondition
        {
            RequestName = requestName,
            Field = fieldPath,
            ExpectedValue = expectedValue,
            Negated = negated,
        };
    }

    private static bool TryParseVariable(string trimmed, ParserState state)
    {
        if (!trimmed.StartsWith("@") || state.InBody)
            return false;

        var eqIndex = trimmed.IndexOf('=');
        if (eqIndex < 0)
            return false;

        var varName = trimmed.Substring(1, eqIndex - 1).Trim();
        var varValue = trimmed.Substring(eqIndex + 1).Trim();

        // Substitute existing variables and functions in the value
        varValue = VariableSubstitutor.SubstituteAll(varValue, state.Variables);

        // Update or add variable
        var existing = false;
        for (int i = 0; i < state.Variables.Count; i++)
        {
            if (state.Variables[i].Name == varName)
            {
                state.Variables[i].Value = varValue;
                existing = true;
                break;
            }
        }
        if (!existing)
        {
            state.Variables.Add(new HttpFileVariable { Name = varName, Value = varValue });
        }

        return true;
    }

    private static bool TryParseAssertion(string trimmed, ParserState state)
    {
        if (state.CurrentRequest == null)
            return false;

        // Strip optional "> " prefix
        var assertionLine = trimmed.StartsWith("> ")
            ? trimmed.Substring(2).Trim()
            : trimmed;

        if (assertionLine.StartsWith("EXPECTED_RESPONSE_STATUS "))
        {
            var value = assertionLine.Substring(24).Trim();
            if (int.TryParse(value, out var statusCode))
                state.CurrentRequest.Assertions.ExpectedStatusCode = statusCode;
            return true;
        }

        if (assertionLine.StartsWith("EXPECTED_RESPONSE_BODY "))
        {
            var value = StripQuotes(assertionLine.Substring(23).Trim());
            state.CurrentRequest.Assertions.ExpectedBody = value;
            return true;
        }

        if (assertionLine.StartsWith("EXPECTED_RESPONSE_HEADER "))
        {
            var header = assertionLine.Substring(24).Trim();
            var colonIndex = header.IndexOf(':');
            if (colonIndex > 0)
            {
                var headerName = header.Substring(0, colonIndex).Trim();
                var headerValue = header.Substring(colonIndex + 1).Trim();
                state.CurrentRequest.Assertions.ExpectedHeaders[headerName] = headerValue;
            }
            return true;
        }

        // Also support EXPECTED_RESPONSE_HEADERS (plural, for compat with httprunner)
        if (assertionLine.StartsWith("EXPECTED_RESPONSE_HEADERS "))
        {
            var value = StripQuotes(assertionLine.Substring(25).Trim());
            var colonIndex = value.IndexOf(':');
            if (colonIndex > 0)
            {
                var headerName = value.Substring(0, colonIndex).Trim();
                var headerValue = value.Substring(colonIndex + 1).Trim();
                state.CurrentRequest.Assertions.ExpectedHeaders[headerName] = headerValue;
            }
            return true;
        }

        return false;
    }

    private static string StripQuotes(string value)
    {
        if (value.Length >= 2 && value[0] == '"' && value[value.Length - 1] == '"')
            return value.Substring(1, value.Length - 2);
        return value;
    }

    private static void FinalizeRequest(ParserState state)
    {
        if (state.CurrentRequest == null)
            return;

        if (state.BodyContent.Length > 0)
        {
            var body = state.BodyContent.ToString().TrimEnd('\r', '\n');
            if (!string.IsNullOrWhiteSpace(body))
                state.CurrentRequest.RequestBody = body;
        }

        state.Requests.Add(state.CurrentRequest);
        state.CurrentRequest = null;
        state.BodyContent.Clear();
        state.InBody = false;
    }

    private class ParserState
    {
        public List<HttpFileRequest> Requests { get; } = new List<HttpFileRequest>();
        public List<HttpFileVariable> Variables { get; } = new List<HttpFileVariable>();
        public HttpFileRequest? CurrentRequest { get; set; }
        public bool InBody { get; set; }
        public StringBuilder BodyContent { get; } = new StringBuilder();
        public bool InIntelliJScript { get; set; }

        // Pending directive state
        public string? PendingName { get; set; }
        public long? PendingTimeout { get; set; }
        public long? PendingConnectionTimeout { get; set; }
        public string? PendingDependsOn { get; set; }
        public List<HttpFileCondition> PendingConditions { get; set; } = new List<HttpFileCondition>();
        public long? PendingPreDelay { get; set; }
        public long? PendingPostDelay { get; set; }

        public void ClearPending()
        {
            PendingName = null;
            PendingTimeout = null;
            PendingConnectionTimeout = null;
            PendingDependsOn = null;
            PendingConditions = new List<HttpFileCondition>();
            PendingPreDelay = null;
            PendingPostDelay = null;
        }
    }
}
