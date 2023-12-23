using Microsoft.CodeAnalysis;

namespace ZeroUnit;

internal static class NameHelper
{
    internal static string GetFullName(INamespaceOrTypeSymbol symbol)
    {
        var parts = new List<string>
        {
            symbol.MetadataName
        };

        ISymbol current = symbol;
        var last = current;

        for (
            current = current.ContainingSymbol;
            current is not INamespaceSymbol { IsGlobalNamespace: true };
            current = current.ContainingSymbol)
        {
            parts.Add(current is ITypeSymbol && last is ITypeSymbol ? "+" : ".");
            parts.Add(current.MetadataName);
        }

        parts.Reverse();

        return string.Concat(parts);
    }
}