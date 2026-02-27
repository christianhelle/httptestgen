using System.Text.RegularExpressions;

namespace HttpTestGen.Core;

public static class TimeoutParser
{
    private static readonly Regex TimeoutRegex = new Regex(
        @"^(\d+)\s*(ms|s|m)?$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static long? Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var match = TimeoutRegex.Match(value.Trim());
        if (!match.Success)
            return null;

        var number = long.Parse(match.Groups[1].Value);
        var unit = match.Groups[2].Value.ToLowerInvariant();

        switch (unit)
        {
            case "s":
                return number * 1000;
            case "m":
                return number * 60 * 1000;
            case "ms":
            case "":
                return number;
            default:
                return null;
        }
    }
}
