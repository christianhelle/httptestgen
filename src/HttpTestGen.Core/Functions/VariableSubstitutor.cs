using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HttpTestGen.Core;

public static class VariableSubstitutor
{
    private static readonly Regex VariableRegex = new Regex(
        @"\{\{([^(}]+)\}\}",
        RegexOptions.Compiled);

    public static string Substitute(string input, IList<HttpFileVariable> variables)
    {
        if (variables == null || variables.Count == 0)
            return input;

        return VariableRegex.Replace(input, match =>
        {
            var varName = match.Groups[1].Value.Trim();
            for (int i = 0; i < variables.Count; i++)
            {
                if (variables[i].Name == varName)
                    return variables[i].Value;
            }
            return match.Value;
        });
    }

    public static string SubstituteAll(string input, IList<HttpFileVariable> variables)
    {
        var result = FunctionSubstitutor.SubstituteFunctions(input);
        result = Substitute(result, variables);
        return result;
    }
}
