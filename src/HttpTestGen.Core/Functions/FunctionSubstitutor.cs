using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HttpTestGen.Core;

public static class FunctionSubstitutor
{
    private static readonly Regex FunctionCallRegex = new Regex(
        @"\{\{(\w+)\(([^)]*)\)\}\}",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly string[] FirstNames =
    {
        "James", "Mary", "Robert", "Patricia", "John", "Jennifer", "Michael", "Linda",
        "David", "Elizabeth", "William", "Barbara", "Richard", "Susan", "Joseph", "Jessica",
        "Thomas", "Sarah", "Christopher", "Karen", "Charles", "Lisa", "Daniel", "Nancy",
        "Matthew", "Betty", "Anthony", "Margaret", "Mark", "Sandra", "Donald", "Ashley",
        "Steven", "Dorothy", "Paul", "Kimberly", "Andrew", "Emily", "Joshua", "Donna",
        "Kenneth", "Michelle", "Kevin", "Carol", "Brian", "Amanda", "George", "Melissa",
        "Timothy", "Deborah"
    };

    private static readonly string[] LastNames =
    {
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis",
        "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson",
        "Thomas", "Taylor", "Moore", "Jackson", "Martin", "Lee", "Perez", "Thompson",
        "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson", "Walker",
        "Young", "Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores",
        "Green", "Adams", "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell",
        "Carter", "Roberts"
    };

    private static readonly string[] EmailDomains =
    {
        "example.com", "test.com", "mail.com", "email.com", "demo.com",
        "sample.org", "testing.net", "mock.io"
    };

    private static readonly string[] Addresses =
    {
        "123 Main St, New York, NY 10001", "456 Oak Ave, Los Angeles, CA 90001",
        "789 Pine Rd, Chicago, IL 60601", "321 Elm Blvd, Houston, TX 77001",
        "654 Maple Dr, Phoenix, AZ 85001", "987 Cedar Ln, Philadelphia, PA 19101",
        "147 Birch Way, San Antonio, TX 78201", "258 Walnut St, San Diego, CA 92101",
        "369 Spruce Ct, Dallas, TX 75201", "741 Ash Pl, San Jose, CA 95101"
    };

    private static readonly string[] JobTitles =
    {
        "Software Engineer", "Product Manager", "Data Scientist", "DevOps Engineer",
        "UX Designer", "QA Engineer", "Technical Lead", "System Architect",
        "Frontend Developer", "Backend Developer", "Full Stack Developer",
        "Database Administrator", "Security Analyst", "Cloud Engineer",
        "Machine Learning Engineer", "Project Manager", "Scrum Master",
        "Business Analyst", "Solutions Architect", "CTO"
    };

    private const string LoremIpsumText =
        "lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor " +
        "incididunt ut labore et dolore magna aliqua ut enim ad minim veniam quis nostrud " +
        "exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat duis aute irure " +
        "dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur " +
        "excepteur sint occaecat cupidatat non proident sunt in culpa qui officia deserunt " +
        "mollit anim id est laborum";

    private static int _counter;

    private static int NextPseudoRandom(int max)
    {
        // Use Guid for randomness (analyzer-safe, no System.Random)
        var bytes = Guid.NewGuid().ToByteArray();
        var value = Math.Abs(BitConverter.ToInt32(bytes, 0));
        return value % max;
    }

    public static string SubstituteFunctions(string input)
    {
        return FunctionCallRegex.Replace(input, match =>
        {
            var funcName = match.Groups[1].Value.ToLowerInvariant();
            var arg = match.Groups[2].Value.Trim().Trim('\'', '"');

            switch (funcName)
            {
                case "guid":
                    return Guid.NewGuid().ToString("N");
                case "string":
                    return GenerateRandomString(20);
                case "number":
                    return NextPseudoRandom(101).ToString();
                case "name":
                    return RandomElement(FirstNames) + " " + RandomElement(LastNames);
                case "first_name":
                    return RandomElement(FirstNames);
                case "last_name":
                    return RandomElement(LastNames);
                case "email":
                    return GenerateEmail();
                case "address":
                    return RandomElement(Addresses);
                case "job_title":
                    return RandomElement(JobTitles);
                case "getdate":
                    return DateTime.Now.ToString("yyyy-MM-dd");
                case "gettime":
                    return DateTime.Now.ToString("HH:mm:ss");
                case "getdatetime":
                    return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                case "getutcdatetime":
                    return DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                case "base64_encode":
                    return Convert.ToBase64String(Encoding.UTF8.GetBytes(arg));
                case "upper":
                    return arg.ToUpperInvariant();
                case "lower":
                    return arg.ToLowerInvariant();
                case "lorem_ipsum":
                    return GenerateLoremIpsum(arg);
                default:
                    return match.Value;
            }
        });
    }

    private static string RandomElement(string[] array)
    {
        return array[NextPseudoRandom(array.Length)];
    }

    private static string GenerateRandomString(int length)
    {
        // Use Guid bytes for randomness
        var sb = new StringBuilder();
        while (sb.Length < length)
        {
            sb.Append(Guid.NewGuid().ToString("N"));
        }
        return sb.ToString().Substring(0, length);
    }

    private static string GenerateEmail()
    {
        var first = RandomElement(FirstNames).ToLowerInvariant();
        var last = RandomElement(LastNames).ToLowerInvariant();
        var domain = RandomElement(EmailDomains);
        return $"{first}.{last}@{domain}";
    }

    private static string GenerateLoremIpsum(string wordCountArg)
    {
        if (!int.TryParse(wordCountArg, out var wordCount) || wordCount <= 0)
            wordCount = 50;

        var words = LoremIpsumText.Split(' ');
        var result = new StringBuilder();
        for (int i = 0; i < wordCount; i++)
        {
            if (i > 0) result.Append(' ');
            result.Append(words[i % words.Length]);
        }
        return result.ToString();
    }
}
