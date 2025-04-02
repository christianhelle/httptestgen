using Microsoft.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace HttpTestGen.SourceGenerator;

[ExcludeFromCodeCoverage]
[Generator(LanguageNames.CSharp)]
public class SourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var httpFiles = context.AdditionalTextsProvider.Where(static file => file.Path.EndsWith(".http"));
        var namesAndContents = httpFiles.Select((text, cancellationToken) =>
        (
            name: Path.GetFileNameWithoutExtension(text.Path),
            content: text.GetText(cancellationToken)!.ToString())
        );

        HttpFileParser httpFileParser = new();
        context.RegisterSourceOutput(
            namesAndContents,
            (sourceProductionContext, nameAndContent) =>
            {
                var requests = httpFileParser.Parse(nameAndContent.content);

                var sb = new StringBuilder();
                sb.AppendLine($"public class {nameAndContent.name}Tests");
                sb.AppendLine("{");
                var i = 0;
                foreach (var request in requests)
                {
                    var requestName = new Uri(request.Endpoint).Host.Replace(".", "_").ToLowerInvariant();
                    sb.AppendLine("    [Xunit.Fact]");
                    sb.AppendLine($"    public async Task {request.Method.ToLowerInvariant()}_{requestName}_{i++}()");
                    sb.AppendLine("    {");
                    sb.AppendLine("        var sut = new System.Net.Http.HttpClient();");
                    sb.AppendLine($"        var response = await sut.{request.Method}Async(\"{request.Endpoint}\");");
                    sb.AppendLine("        Xunit.Assert.True(response.IsSuccessStatusCode);");
                    sb.AppendLine("    }");
                }
                sb.AppendLine("}");

                sourceProductionContext.AddSource(
                    $"{nameAndContent.name}.http",
                    sb.ToString()
                );
            });
    }
}
