using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace PacketSourceGenerator;


[Generator(LanguageNames.CSharp)]
public class OutgoingPacketGenerator : IIncrementalGenerator
{
    /*    private static string GenerateShit(ImmutableArray<(RecordDeclarationSyntax, ITypeSymbol)> stuff)
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
        }*/
    private string GenerateWriteImpl(ITypeSymbol typeSymbol, ParameterListSyntax paramListSyntax)
    {
        return $@"using Common.Utilities.Net;
namespace {typeSymbol.ContainingNamespace};
partial record struct {typeSymbol.Name}
{{
    public readonly void Write(NetworkWriter wtr)
    {{
{GenerateMethods(paramListSyntax)}
/*{debugStuff}*/
    }}
}}";
    }
    private string GeneratePacketIdImpl(ITypeSymbol typeSymbol)
    {
        return $@"namespace {typeSymbol.ContainingNamespace};
partial record struct {typeSymbol.Name}
{{
    static PacketId IOutgoingPacket.PacketId => PacketId.{typeSymbol.Name.ToUpper()};
}}";
    }

    private static string GenerateMethods(ParameterListSyntax typeSymbol)
    {
        var sb = new StringBuilder();

        foreach (var fieldSymbol in typeSymbol.Parameters)
        {
            var propName = fieldSymbol.Identifier.Text;
            sb.AppendLine($"\t\twtr.Write({propName});");
        }
        return sb.ToString().TrimEnd();
    }
    public void Execute(SourceProductionContext context,
      ImmutableArray<TargetData> source)
    {
        foreach (var item in source)
        {
            if (!item.HasWrite)
            {
                var code = GenerateWriteImpl(item.Symbol, item.Syntax.ParameterList);
                context.AddSource($"{item.Symbol.Name}.Write.cs", SourceText.From(code, Encoding.UTF8));
            }
            if (!item.HasPacketId)
            {
                var code = GeneratePacketIdImpl(item.Symbol);
                context.AddSource($"{item.Symbol.Name}.PacketId.cs", SourceText.From(code, Encoding.UTF8));
            }
        }
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        debugStuff = new();
        var pipeline =
            context.SyntaxProvider.CreateSyntaxProvider( // A
                (node, _) => node is RecordDeclarationSyntax rec &&
                rec.ParameterList?.Parameters.Count > 0, // B
                (syntax, _) => GetSemanticTargetForGeneration(syntax)) // C
                .Where(n => n is not null).Collect();

        context.RegisterSourceOutput(pipeline, Execute);
    }
    static StringBuilder debugStuff = new();
    static TargetData GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var syn = (RecordDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(syn) is not ITypeSymbol symbol)
        {
            debugStuff.AppendLine($"Context is {context.SemanticModel.GetSymbolInfo(syn).Symbol?.GetType().AssemblyQualifiedName ?? "null"}");
            // weird, we couldn't get the symbol, ignore it
            return null;
        }
        var inter = context.SemanticModel.Compilation.GetTypeByMetadataName("GameServer.Game.Network.Messaging.IOutgoingPacket");
        if (symbol.Interfaces.Contains(inter))
        {
            var write = symbol.GetMembers("Write");
            var packetIdImpl = symbol.GetMembers("GameServer.Game.Network.Messaging.IOutgoingPacket.PacketId");
            //
            //static PacketId IOutgoingPacket.PacketId => ;
            //foreach (var item in bonus)
            //{
            //    debugStuff.AppendLine(item.Name);
            //}

            return new(syn, symbol, write.Length > 0, packetIdImpl.Length > 0);
        }

        return null;
    }

}
public class TargetData(RecordDeclarationSyntax syntax, ITypeSymbol symbol, bool hasWriteImpl, bool hasPacketIdImpl)
{
    public readonly RecordDeclarationSyntax Syntax = syntax;
    public readonly ITypeSymbol Symbol = symbol;
    public readonly bool HasWrite = hasWriteImpl;
    public readonly bool HasPacketId = hasPacketIdImpl;
}