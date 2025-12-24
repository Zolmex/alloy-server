using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace PacketSourceGenerator;

[Generator(LanguageNames.CSharp)]
public class OutgoingPacketGenerator : IIncrementalGenerator
{
    private static readonly Dictionary<string, string> types = new()
    {
        { "string", "String" },
        { "sbyte", "SByte" },
        { "byte", "Byte" },
        { "short", "Int16" },
        { "ushort", "UInt16" },
        { "int", "Int32" },
        { "uint", "UInt32" },
        { "long", "Int64" },
        { "ulong", "UInt64" },
        { "char", "Char" },
        { "float", "Single" },
        { "double", "Double" },
        { "bool", "Boolean" },
        { "decimal", "Decimal" }
    };

    private static StringBuilder debugStuff = new();

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        debugStuff = new StringBuilder();
        var pipeline =
            context.SyntaxProvider.CreateSyntaxProvider( // A
                    (node, _) => node is RecordDeclarationSyntax rec &&
                                 rec.ParameterList?.Parameters.Count > 0, // B
                    (syntax, _) => GetSemanticTargetForGeneration(syntax)) // C
                .Where(n => n is not null).Collect();

        context.RegisterSourceOutput(pipeline, Execute);
    }

    private static string GenerateShit(ImmutableArray<TargetData> stuff)
    {
        var str = new StringBuilder();
        str.Append(debugStuff);
        //foreach (var item in stuff)
        //{
        //    var nod = item.Syntax;
        //    str.AppendLine(GenerateMethods(nod.ParameterList));
        //}
        if (stuff.Length == 0)
        {
            str.AppendLine("Stuff length is 0 dawg.");
        }

        return str.ToString();
    }

    private string GenerateWriteImpl(ITypeSymbol typeSymbol, ParameterListSyntax paramListSyntax)
    {
        return $@"
using Common;
using Common.Network;
namespace {typeSymbol.ContainingNamespace};
partial record struct {typeSymbol.Name}
{{
    public readonly void Write(NetworkWriter wtr)
    {{
{GenerateWriteMethods(paramListSyntax)}
/*{debugStuff}*/
    }}
}}";
    }

    private string GenerateReadImpl(ITypeSymbol typeSymbol, ParameterListSyntax paramListSyntax)
    {
        return $@"
using Common;
using Common.Network;
namespace {typeSymbol.ContainingNamespace};
partial record struct {typeSymbol.Name}
{{
    public static {typeSymbol.Name} Read(NetworkReader r)
    {{
        return new {typeSymbol.Name}({GenerateReadMethods(paramListSyntax)});
    }}
}}";
    }

    private string GeneratePacketIdImpl(ITypeSymbol typeSymbol)
    {
        return $@"namespace {typeSymbol.ContainingNamespace};
partial record struct {typeSymbol.Name}
{{
    public static PacketId PacketId => PacketId.{typeSymbol.Name.ToUpper()};
}}";
    }

    private static string GenerateWriteMethods(ParameterListSyntax typeSymbol)
    {
        var sb = new StringBuilder();

        foreach (var fieldSymbol in typeSymbol.Parameters)
        {
            var propName = fieldSymbol.Identifier.Text;
            sb.AppendLine($"\t\twtr.Write({propName});");
        }

        return sb.ToString().TrimEnd();
    }

    private static string GenerateReadMethods(ParameterListSyntax typeSymbol)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < typeSymbol.Parameters.Count; i++)
        {
            var fieldSymbol = typeSymbol.Parameters[i];
            var type = fieldSymbol.Type.ToString();
            var str = $"<{type.Replace("[]", null)}>";
            if (types.TryGetValue(type, out var fullName))
            {
                str = char.ToUpper(fullName[0]) + fullName.Substring(1);
            }
            else if (!type.Contains("[]"))
            {
                str = char.ToUpper(type[0]) + type.Substring(1);
            }

            sb.Append($"r.Read{str}()");
            if (i != typeSymbol.Parameters.Count - 1)
                sb.Append(", ");
        }

        return sb.ToString().TrimEnd();
    }

    public void Execute(SourceProductionContext context,
        ImmutableArray<TargetData> source)
    {
        /*        var debugCode = GenerateShit(source);
                context.AddSource("DebugStuff.txt", SourceText.From(debugCode, Encoding.UTF8));
        */
        foreach (var item in source)
        {
            if (!item.HasWrite)
            {
                var code = GenerateWriteImpl(item.Symbol, item.Syntax.ParameterList);
                context.AddSource($"OutgoingWrites/{item.Symbol.Name}.Write.cs", SourceText.From(code, Encoding.UTF8));
            }

            if (!item.HasRead)
            {
                var code = GenerateReadImpl(item.Symbol, item.Syntax.ParameterList);
                context.AddSource($"OutgoingReads/{item.Symbol.Name}.Read.cs", SourceText.From(code, Encoding.UTF8));
            }

            if (!item.HasPacketId)
            {
                var code = GeneratePacketIdImpl(item.Symbol);
                context.AddSource($"OutgoingPacketIds/{item.Symbol.Name}.PacketId.cs", SourceText.From(code, Encoding.UTF8));
            }
        }
    }

    private static TargetData GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var syn = (RecordDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(syn) is not ITypeSymbol symbol)
        {
            debugStuff.AppendLine($"Context is {context.SemanticModel.GetSymbolInfo(syn).Symbol?.GetType().AssemblyQualifiedName ?? "null"}");
            // weird, we couldn't get the symbol, ignore it
            return null;
        }

        var inter = context.SemanticModel.Compilation.GetTypeByMetadataName("GameServer.Game.Network.Messaging.IOutgoingPacket`1");
        //foreach(var i in symbol.Interfaces)
        //{
        //    debugStuff.AppendLine(i.MetadataName);
        //    if (symbol.Interfaces.Any(n => n.MetadataName == "IOutgoingPacket`1"))
        //        debugStuff.AppendLine("I CONTAIN INTER");
        //}
        //debugStuff.AppendLine("");
        //return new(syn, symbol, true, true);

        if (symbol.Interfaces.Any(n => n.MetadataName == "IOutgoingPacket`1"))
        {
            var write = symbol.GetMembers("Write");
            var read = symbol.GetMembers("Read");
            var packetIdImpl = symbol.GetMembers("GameServer.Game.Network.Messaging.IOutgoingPacket.PacketId");
            //
            //static PacketId IOutgoingPacket.PacketId => ;
            //foreach (var item in bonus)
            //{
            //    debugStuff.AppendLine(item.Name);
            //}

            return new TargetData(syn, symbol, write.Length > 0, read.Length > 0, packetIdImpl.Length > 0);
        }

        return null;
    }
}

public class TargetData(RecordDeclarationSyntax syntax, ITypeSymbol symbol, bool hasWriteImpl, bool hasReadImpl, bool hasPacketIdImpl)
{
    public readonly bool HasPacketId = hasPacketIdImpl;
    public readonly bool HasRead = hasReadImpl;
    public readonly bool HasWrite = hasWriteImpl;
    public readonly ITypeSymbol Symbol = symbol;
    public readonly RecordDeclarationSyntax Syntax = syntax;
}