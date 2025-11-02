using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace GameServer.Game.Network.Messaging;

[Generator]
public class OutgoingPacketGenerator : ISourceGenerator
{
    private string GeneratePropertyChanged(ITypeSymbol typeSymbol, ParameterListSyntax paramListSyntax)
    {
        return $@"
    using Common.Utilities.Net;
    namespace {typeSymbol.ContainingNamespace}
    {{
      partial struct {typeSymbol.Name}
      {{
          {GenerateMethods(paramListSyntax)}
      }}
    }}";
    }
    private static string GenerateMethods(ParameterListSyntax typeSymbol)
    {
        var sb = new StringBuilder();

        sb.AppendLine($@"
    public readonly void Write(NetworkWriter wtr) {{");

        foreach (var fieldSymbol in typeSymbol.Parameters)
        {
            var propName = fieldSymbol.Identifier.Text;
            sb.AppendLine($"wtr.Write({propName});");
        }
        sb.AppendLine("}");

        return sb.ToString();
    }
    public void Execute(GeneratorExecutionContext context)
    {
        var compilation = context.Compilation;

        var interfaceType = compilation.GetTypeByMetadataName(typeof(IOutgoingPacket).FullName);

        foreach (var syntaxTree in compilation.SyntaxTrees)
        {
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var nodes = syntaxTree.GetRoot().DescendantNodesAndSelf().OfType<RecordDeclarationSyntax>();
            var records = nodes.Where(n => n.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.RecordStructDeclaration));

            var set = records
              .Where(n => n.ParameterList != null); // Find primary constructors
            var symbolsSet = set
            .Select(n => semanticModel.GetDeclaredSymbol(n))
            .OfType<ITypeSymbol>()
            .Where(n => n.Interfaces.Contains(interfaceType)).ToImmutableHashSet();

            foreach (var typeSymbol in symbolsSet)
            {
                var source = GeneratePropertyChanged(typeSymbol, );
                context.AddSource($"{typeSymbol.Name}.Write.cs", source);
            }
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        // Register a syntax receiver that will be created for each generation pass
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }



    /// <summary>
    /// Created on demand before each generation pass
    /// </summary>
    class SyntaxReceiver : ISyntaxReceiver
    {
        public List<FieldDeclarationSyntax> CandidateFields { get; } = new List<FieldDeclarationSyntax>();

        /// <summary>
        /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
        /// </summary>
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            // any field with at least one attribute is a candidate for property generation
            if (syntaxNode is FieldDeclarationSyntax fieldDeclarationSyntax
                && fieldDeclarationSyntax.AttributeLists.Count > 0)
            {
                CandidateFields.Add(fieldDeclarationSyntax);
            }
        }
    }
}