using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace PacketSourceGenerator;

[Generator(LanguageNames.CSharp)]
internal class IncomingPacketGenerator : IIncrementalGenerator
{
    private const string FolderPrefix = "Incoming";
    static readonly Dictionary<string, string> types = new()
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

    private static string GenerateShit(ImmutableArray<TargetData> stuff)
    {
        var str = new StringBuilder();
        str.Append(debugStuff.ToString());
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

    private string GenerateWriteImpl(ITypeSymbol symbol, ImmutableArray<FieldDeclarationSyntax> members)
    {
        return $@"using Common.Utilities.Net;
using Common;
namespace {symbol.ContainingNamespace};
partial record {symbol.Name}
{{
    public void Write(NetworkWriter wtr)
    {{
{GenerateWriteMethods(members)}
    }}
}}";
    }

    private string GenerateReadImpl(ITypeSymbol typeSymbol, ImmutableArray<FieldDeclarationSyntax> paramListSyntax)
    {
        return $@"using Common.Utilities.Net;
using Common;
namespace {typeSymbol.ContainingNamespace};
partial record {typeSymbol.Name}
{{
    public void Read(NetworkReader r)
    {{
{GenerateReadMethods(paramListSyntax)}
    }}
}}";
    }
    private string GeneratePacketIdImpl(ITypeSymbol typeSymbol)
    {
        return $@"namespace {typeSymbol.ContainingNamespace};
partial record {typeSymbol.Name}
{{
    public static PacketId PacketId => PacketId.{typeSymbol.Name.ToUpper()};
}}";
    }

    private static string GenerateWriteMethods(ImmutableArray<FieldDeclarationSyntax> members)
    {
        var sb = new StringBuilder();

        foreach (var fieldSymbol in members)
        {
            foreach (var propName in fieldSymbol.Declaration.Variables)
                sb.AppendLine($"\t\twtr.Write({propName.Identifier.Text});");
        }
        return sb.ToString().TrimEnd();
    }
    private static string GenerateReadMethods(ImmutableArray<FieldDeclarationSyntax> typeSymbol)
    {
        var sb = new StringBuilder();

        foreach (var fieldSymbol in typeSymbol)
        {
            var type = fieldSymbol.Declaration.Type.ToString();
            foreach (var propName in fieldSymbol.Declaration.Variables)
            {
                var name = propName.Identifier.Text.ToString();
                string str = $"<{type.Replace("[]", null)}>";
                if (types.TryGetValue(type, out var fullName))
                {
                    str = char.ToUpper(fullName[0]) + fullName.Substring(1);
                }
                else if (!type.Contains("[]"))
                {
                    str = char.ToUpper(type[0]) + type.Substring(1);
                }
                sb.AppendLine($"\t\t{name} = r.Read{str}();");
            }
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
                var code = GenerateWriteImpl(item.Symbol, item.Fields);
                context.AddSource($"{FolderPrefix}Writes/{item.Symbol.Name}.Write.cs", SourceText.From(code, Encoding.UTF8));
            }
            if (!item.HasRead)
            {
                var code = GenerateReadImpl(item.Symbol, item.Fields);
                context.AddSource($"{FolderPrefix}Reads/{item.Symbol.Name}.Read.cs", SourceText.From(code, Encoding.UTF8));
            }
            if (!item.HasPacketId)
            {
                var code = GeneratePacketIdImpl(item.Symbol);
                context.AddSource($"{FolderPrefix}PacketIds/{item.Symbol.Name}.PacketId.cs", SourceText.From(code, Encoding.UTF8));
            }
        }
    }


    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        debugStuff = new();
        var pipeline =
            context.SyntaxProvider.CreateSyntaxProvider( // A
                (node, _) => node is RecordDeclarationSyntax rec, // B
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
        var inter = context.SemanticModel.Compilation.GetTypeByMetadataName("GameServer.Game.Network.Messaging.IIncomingPacket");
        //foreach(var i in symbol.Interfaces)
        //{
        //    debugStuff.AppendLine(i.MetadataName);
        //    if (symbol.Interfaces.Any(n => n.MetadataName == "IOutgoingPacket`1"))
        //        debugStuff.AppendLine("I CONTAIN INTER");
        //}
        //debugStuff.AppendLine("");
        //return new(syn, symbol, true, true);

        if (symbol.Interfaces.Contains(inter))
        {
            var write = symbol.GetMembers("Write");
            var read = symbol.GetMembers("Read");
            var packetIdImpl = symbol.GetMembers("GameServer.Game.Network.Messaging.IIncomingPacket.PacketId");
            //
            //static PacketId IOutgoingPacket.PacketId => ;
            //foreach (var item in bonus)
            //{
            //    debugStuff.AppendLine(item.Name);
            //}

            return new(syn, symbol, write.Length > 0, read.Length > 0, packetIdImpl.Length > 0);
        }

        return null;
    }

    public class TargetData(RecordDeclarationSyntax syntax, ITypeSymbol symbol, bool hasWriteImpl, bool hasReadImpl, bool hasPacketIdImpl)
    {
        public readonly RecordDeclarationSyntax Syntax = syntax;
        public readonly ImmutableArray<FieldDeclarationSyntax> Fields = syntax.Members.Where(n => n.Modifiers.FirstOrDefault().Text == "public").OfType<FieldDeclarationSyntax>().ToImmutableArray();
        public readonly ITypeSymbol Symbol = symbol;
        public readonly bool HasWrite = hasWriteImpl;
        public readonly bool HasRead = hasReadImpl;
        public readonly bool HasPacketId = hasPacketIdImpl;
    }
}