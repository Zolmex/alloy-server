using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace PacketSourceGenerator;

[Generator(LanguageNames.CSharp)]
internal class HelloGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // get the additional text provider
        IncrementalValuesProvider<AdditionalText> additionalTexts = context.AdditionalTextsProvider;

        // apply a 1-to-1 transform on each text, extracting the path
        IncrementalValuesProvider<string> transformed = additionalTexts.Select(static (text, _) => text.Path);

        // collect the paths into a batch
        IncrementalValueProvider<ImmutableArray<string>> collected = transformed.Collect();

        // take the file paths from the above batch and make some user visible syntax
        context.RegisterSourceOutput(collected, static (sourceProductionContext, filePaths) =>
        {
            sourceProductionContext.AddSource("additionalFiles.cs", @"
namespace Generated
{
    public class AdditionalTextList
    {
        public static void PrintTexts()
        {
            System.Console.WriteLine(""Additional Texts were: " + string.Join(", ", filePaths) + @" "");
        }
    }
}");
        });
    }
}
