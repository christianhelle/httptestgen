using Microsoft.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;
using HttpTestGen.Core;

namespace HttpTestGen.XunitGenerator;

[ExcludeFromCodeCoverage]
[Generator(LanguageNames.CSharp)]
public class SourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var httpFiles = context.AdditionalTextsProvider.Where(static file => file.Path.EndsWith(".http"));
        var namesAndContents = httpFiles.Select(
            (text, cancellationToken) =>
            (
                name: Path.GetFileNameWithoutExtension(text.Path),
                content: text.GetText(cancellationToken)!.ToString())
        );

        HttpFileParser httpFileParser = new();
        context.RegisterSourceOutput(
            namesAndContents,
            (sourceProductionContext, nameAndContent) =>
            {
                GenerateHttpTests(
                    sourceProductionContext,
                    nameAndContent.name,
                    nameAndContent.content,
                    httpFileParser);
            });
    }

    private static void GenerateHttpTests(
        SourceProductionContext sourceProductionContext,
        string httpFilename,
        string httpFileContents,
        HttpFileParser httpFileParser) =>
        sourceProductionContext.AddSource(
            $"{httpFilename}.Xunit.http",
            new XunitTestGenerator()
                .Generate(
                    httpFilename,
                    [.. httpFileParser.Parse(httpFileContents)]));
}