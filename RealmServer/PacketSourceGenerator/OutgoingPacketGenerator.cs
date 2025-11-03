using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace PacketSourceGenerator;


[Generator(LanguageNames.CSharp)]
public class OutgoingPacketGenerator : IIncrementalGenerator
{
    private static string GenerateShit(ImmutableArray<(RecordDeclarationSyntax, ITypeSymbol)> stuff)
    {
        var str = new StringBuilder();
        str.Append(debugStuff.ToString());
        foreach (var item in stuff)
        {
            var nod = item.Item1;
            str.AppendLine(GenerateMethods(nod.ParameterList));
        }
        if (stuff.Length == 0)
        {
            str.AppendLine("Stuff length is 0 dawg.");
        }
        return str.ToString();
    }
    private string GeneratePropertyChanged(ITypeSymbol typeSymbol, ParameterListSyntax paramListSyntax)
    {
        return $@"
    using Common.Utilities.Net;
    namespace {typeSymbol.ContainingNamespace}
    {{
      partial record struct {typeSymbol.Name}
      {{
            public readonly void Write(NetworkWriter wtr)
            {{
                {GenerateMethods(paramListSyntax)}
            }}
      }}
    }}";
    }
    private static string GenerateMethods(ParameterListSyntax typeSymbol)
    {
        var sb = new StringBuilder();

        foreach (var fieldSymbol in typeSymbol.Parameters)
        {
            var propName = fieldSymbol.Identifier.Text;
            sb.AppendLine($"wtr.Write({propName});");
        }

        return sb.ToString();
    }
    public void Execute(SourceProductionContext context,
      ImmutableArray<(RecordDeclarationSyntax, ITypeSymbol)> source)
    {
        foreach (var item in source)
        {
            var code = GeneratePropertyChanged(item.Item2, item.Item1.ParameterList);
            context.AddSource($"{item.Item2.Name}.Write.cs", SourceText.From(code, Encoding.UTF8));
        }
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var pipeline =
            context.SyntaxProvider.CreateSyntaxProvider( // A
                (node, _) => node is RecordDeclarationSyntax rec &&
                rec.ParameterList?.Parameters.Count > 0, // B
                (syntax, _) => GetSemanticTargetForGeneration(syntax)) // C
                .Where(n => n.Item1 is not null).Collect();

        //        context.RegisterSourceOutput(pipeline, static (sourceProductionContext, filePaths) =>
        //        {
        //            sourceProductionContext.AddSource("additionalFiles.cs", @$"
        //namespace PacketGen
        //{{
        //    public class AdditionalTextList
        //    {{
        //        public static void PrintTexts()
        //        {{
        //            System.Console.WriteLine(""Additional Texts were: {string.Join(", ", filePaths.Length)}"");
        ///*
        //{GenerateShit(filePaths)}
        //*/
        //        }}
        //    }}
        //}}");
        //        });
        context.RegisterSourceOutput(pipeline, Execute);
    }
    static StringBuilder debugStuff = new();
    static (RecordDeclarationSyntax, ITypeSymbol) GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var syn = (RecordDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(syn) is not ITypeSymbol symbol)
        {
            debugStuff.AppendLine($"Context is {context.SemanticModel.GetSymbolInfo(syn).Symbol?.GetType().AssemblyQualifiedName ?? "null"}");
            // weird, we couldn't get the symbol, ignore it
            return (null, null);
        }
        var inter = context.SemanticModel.Compilation.GetTypeByMetadataName("GameServer.Game.Network.Messaging.IOutgoingPacket");
        if (symbol.Interfaces.Contains(inter))
        {
            return (syn, symbol);
        }

        return (null, null);
    }
}