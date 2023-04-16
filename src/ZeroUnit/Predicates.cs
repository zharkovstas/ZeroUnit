using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ZeroUnit;

internal static class Predicates
{
    internal static bool IsTestMethod(MethodDeclarationSyntax method)
    {
        return IsVoidOrTask(method.ReturnType)
            && method.ParameterList.Parameters.Count == 0
            && method.Parent is ClassDeclarationSyntax @class
            && method.Modifiers.Any(SyntaxKind.PublicKeyword)
            && @class.Modifiers.Any(SyntaxKind.PublicKeyword);
    }

    internal static bool IsTestMethod(IMethodSymbol method)
    {
        if (!method.IsStatic && !CanInstantiate(method.ContainingType))
        {
            return false;
        }

        return !method.IsGenericMethod
            && method.Name != "Dispose";
    }

    private static bool IsVoidOrTask(TypeSyntax returnType)
    {
        return returnType switch
        {
            PredefinedTypeSyntax predefinedType => predefinedType.Keyword.IsKind(SyntaxKind.VoidKeyword),
            IdentifierNameSyntax identifierName => identifierName.Identifier.Text == "Task",
            QualifiedNameSyntax qualifiedName => qualifiedName.ToString() == "System.Threading.Tasks.Task",
            _ => false
        };
    }

    private static bool CanInstantiate(INamedTypeSymbol type)
    {
        return !type.IsAbstract && !type.IsGenericType && type.InstanceConstructors.Any(
            x => x.Parameters.Length == 0 && x.DeclaredAccessibility == Accessibility.Public
        );
    }
}