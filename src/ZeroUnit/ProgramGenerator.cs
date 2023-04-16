using System.Collections.Immutable;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ZeroUnit;

[Generator]
public class ProgramGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var methods = context.SyntaxProvider
            .CreateSyntaxProvider(IsTestMethod, GetTestMethod)
            .Where(method => method is not null)
            .Collect();

        context.RegisterSourceOutput(methods, GenerateCode!);
    }

    private static bool IsTestMethod(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is MethodDeclarationSyntax method && Predicates.IsTestMethod(method);
    }

    private static bool IsVoid(TypeSyntax returnType)
    {
        return returnType switch
        {
            PredefinedTypeSyntax predefinedType => predefinedType.Keyword.IsKind(SyntaxKind.VoidKeyword),
            IdentifierNameSyntax identifierName => identifierName.Identifier.Text == "Task",
            QualifiedNameSyntax qualifiedName => qualifiedName.ToString() == "System.Threading.Tasks.Task",
            _ => false
        };
    }

    private static IMethodSymbol? GetTestMethod(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        return context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken) as IMethodSymbol;
    }

    private static void GenerateCode(
        SourceProductionContext context,
        ImmutableArray<IMethodSymbol> methods)
    {
        context.AddSource("Program.cs", ProgramCodeGenerator.Generate(methods));
    }
}