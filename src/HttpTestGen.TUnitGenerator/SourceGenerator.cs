using HttpTestGen.Core;
using Microsoft.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace HttpTestGen.TUnitGenerator;

[ExcludeFromCodeCoverage]
[Generator(LanguageNames.CSharp)]
public class SourceGenerator : IIncrementalGenerator
{
    [SuppressMessage(
        "MicrosoftCodeAnalysisCorrectness",
        "RS1035:Do not use APIs banned for analyzers",
        Justification = "TUnit uses source generators and it's not possible to chain them")]
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var httpFiles = context.AdditionalTextsProvider.Where(static file => file.Path.EndsWith(".http"));
        var files = httpFiles.Select(
            (text, cancellationToken) =>
            (
                path: text.Path,
                name: Path.GetFileNameWithoutExtension(text.Path),
                content: text.GetText(cancellationToken)!.ToString()
            )
        );

        HttpFileParser httpFileParser = new();
        TUnitTestGenerator generator = new();

        context.RegisterSourceOutput(
            files,
            (context, file) =>
            {
                var code = generator.Generate(
                    file.name,
                    [.. httpFileParser.Parse(file.content)]);

                var folder = Path.GetDirectoryName(file.path)!;
                var fileName = Path.Combine(folder, $"{file.name}.http.cs");
                File.WriteAllText(fileName, code);
            });
    }
}