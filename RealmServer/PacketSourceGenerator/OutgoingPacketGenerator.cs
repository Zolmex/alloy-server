using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace PacketSourceGenerator;


[Generator]
public class OutgoingPacketGenerator : IIncrementalGenerator
{
    private string GeneratePropertyChanged(ITypeSymbol typeSymbol, ParameterListSyntax paramListSyntax)
    {
        return $@"
    using Common.Utilities.Net;
    namespace {typeSymbol.ContainingNamespace}
    {{
      partial struct {typeSymbol.Name}
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
      (RecordDeclarationSyntax, ITypeSymbol) source)
    {

        var code = GeneratePropertyChanged(source.Item2, source.Item1.ParameterList);
        context.AddSource($"{source.Item2.Name}.Write.cs", SourceText.From(code, Encoding.UTF8));
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var pipeline =
            context.SyntaxProvider.CreateSyntaxProvider( // A
                (node, _) => node is RecordDeclarationSyntax rec && rec.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.RecordStructDeclaration) && rec.ParameterList?.Parameters.Count > 0, // B
                (syntax, _) => GetSemanticTargetForGeneration(syntax)) // C
                ;
        context.RegisterSourceOutput(pipeline, Execute);
    }
    static (RecordDeclarationSyntax, ITypeSymbol) GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var syn = (RecordDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetSymbolInfo(syn).Symbol is not ITypeSymbol symbol)
        {
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