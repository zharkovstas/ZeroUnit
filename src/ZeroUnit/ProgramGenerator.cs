using System.Collections.Immutable;

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
            .Where(method => method is not null);

        var entryPoint = context.CompilationProvider
            .Select((c, ct) => c.GetEntryPoint(ct));

        var provider = entryPoint.Combine(methods.Collect());

        context.RegisterSourceOutput(
            provider,
            static (context, x) => GenerateCode(context, x.Left, x.Right));
    }

    private static bool IsTestMethod(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is MethodDeclarationSyntax method && Predicates.IsTestMethod(method);
    }

    private static IMethodSymbol? GetTestMethod(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        return context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken) as IMethodSymbol;
    }

    private static void GenerateCode(
        SourceProductionContext context,
        IMethodSymbol? entryPoint,
        ImmutableArray<IMethodSymbol?> methods)
    {
        context.AddSource("Program.g.cs", ProgramCodeGenerator.Generate(entryPoint, methods));
    }
}