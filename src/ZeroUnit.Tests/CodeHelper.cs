using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ZeroUnit.Tests;

internal static class CodeHelper
{
    internal static IMethodSymbol[] GetMethodSymbols(string code)
    {
        var tree = CSharpSyntaxTree.ParseText(code);
        var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);

        var compilation = CSharpCompilation.Create(
            "TestCompilation",
            syntaxTrees: new[] { tree },
            references: new[] { mscorlib });

        var model = compilation.GetSemanticModel(tree);

        return tree
            .GetRoot()
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .Single()
            .Members
            .OfType<MethodDeclarationSyntax>()
            .Select(x => model.GetDeclaredSymbol(x))
            .OfType<IMethodSymbol>()
            .ToArray();
    }
}