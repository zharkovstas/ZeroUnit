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
        return node is MethodDeclarationSyntax method
            && method.ReturnType is PredefinedTypeSyntax returnType
            && returnType.Keyword.IsKind(SyntaxKind.VoidKeyword)
            && method.ParameterList.Parameters.Count == 0
            && method.Parent is ClassDeclarationSyntax @class
            && method.Modifiers.Any(SyntaxKind.PublicKeyword)
            && @class.Modifiers.Any(SyntaxKind.StaticKeyword)
            && @class.Modifiers.Any(SyntaxKind.PublicKeyword);
    }

    private static IMethodSymbol? GetTestMethod(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        return context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken) as IMethodSymbol;
    }

    private static void GenerateCode(
        SourceProductionContext context,
        ImmutableArray<IMethodSymbol> methods)
    {
        var sb = new StringBuilder();
        sb.AppendLine("internal sealed class Program");
        sb.AppendLine("{");
        sb.AppendLine("    private static void Main(string[] args)");
        sb.AppendLine("    {");
        sb.AppendLine("#pragma warning disable CA1303");
        sb.AppendLine("#pragma warning disable CA1031");
        foreach (var method in methods)
        {
            sb.AppendLine("        try");
            sb.AppendLine("        {");
            var methodFullName = $"{GetFullMetadataName(method.ContainingType)}.{method.Name}";
            sb.AppendLine($"                {methodFullName}();");
            sb.AppendLine(@$"                Console.WriteLine(""[v] {methodFullName}"");");
            sb.AppendLine("        }");
            sb.AppendLine("        catch (Exception ex)");
            sb.AppendLine("        {");
            sb.AppendLine(@$"                Console.WriteLine($""[x] {methodFullName}: {{ex.Message}}"");");
            sb.AppendLine("        }");
        }
        sb.AppendLine("#pragma warning disable CA1031");
        sb.AppendLine("#pragma warning restore CA1303");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        context.AddSource("Program.cs", sb.ToString());
    }

    private static string GetFullMetadataName(INamespaceOrTypeSymbol symbol)
    {
        ISymbol s = symbol;
        var sb = new StringBuilder(s.MetadataName);

        var last = s;
        s = s.ContainingSymbol;
        while (!IsRootNamespace(s))
        {
            if (s is ITypeSymbol && last is ITypeSymbol)
            {
                sb.Insert(0, '+');
            }
            else
            {
                sb.Insert(0, '.');
            }
            sb.Insert(0, s.MetadataName);
            s = s.ContainingSymbol;
        }

        return sb.ToString();
    }

    private static bool IsRootNamespace(ISymbol s)
    {
        return s is INamespaceSymbol && ((INamespaceSymbol)s).IsGlobalNamespace;
    }
}