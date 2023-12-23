using Microsoft.CodeAnalysis;

namespace ZeroUnit;

internal static class ProgramCodeGenerator
{
    internal static string Generate(
        IMethodSymbol? mainMethod,
        IReadOnlyList<IMethodSymbol> methods)
    {
        var program = mainMethod?.ContainingType;
        if (program is not null && !Predicates.IsPartial(program))
        {
            return string.Empty;
        }

        var sb = new CodeStringBuilder();

        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Concurrent;");
        sb.AppendLine("using System.Threading;");
        sb.AppendLine("using System.Threading.Tasks;");
        sb.AppendLine();

        var ns = mainMethod?.ContainingNamespace;

        if (ns is { IsGlobalNamespace: false })
        {
            sb.AppendLine($"namespace {ns.ToDisplayString()}");
            sb.AppendLine("{");
            sb.Indent();
        }

        var testMethods = methods
            .Where(Predicates.IsTestMethod)
            .ToArray();

        GenerateClass(sb, program, mainMethod, testMethods);

        if (ns is { IsGlobalNamespace: false })
        {
            sb.Unindent();
            sb.AppendLine("}");
        }

        return sb.ToString();
    }

    private static void GenerateClass(
        CodeStringBuilder sb,
        INamedTypeSymbol? program,
        IMethodSymbol? mainMethod,
        IMethodSymbol[] testMethods)
    {
        if (program is not null)
        {
            GeneratePartialClassHeader(sb, program);
        }
        else
        {
            sb.AppendLine($"internal static class Program");
        }

        sb.AppendLine("{");
        sb.Indent();

        if (mainMethod is null)
        {
            sb.AppendLine("public static Task<int> Main()");
            sb.AppendLine("{");
            sb.AppendLine("    return RunTestsAsync();");
            sb.AppendLine("}");
            sb.AppendLine();
        }

        var existingRunTestsMethod = program?
            .GetMembers("RunTestsAsync")
            .OfType<IMethodSymbol>()
            .FirstOrDefault(x => x.Parameters.Length == 0);

        if (testMethods.Length > 0)
        {
            RunTestsMethodGenerator.Generate(sb, existingRunTestsMethod, testMethods);
        }
        else
        {
            RunTestsMethodGenerator.GenerateStub(sb, existingRunTestsMethod);
        }

        sb.Unindent();
        sb.AppendLine("}");
    }

    private static void GeneratePartialClassHeader(CodeStringBuilder sb, INamedTypeSymbol program)
    {
        if (program.DeclaredAccessibility == Accessibility.Public)
        {
            sb.StartLine("public ");
        }
        else
        {
            sb.StartLine("internal ");
        }
        if (program.IsStatic)
        {
            sb.Append("static ");
        }
        sb.EndLine($"partial class {program.Name}");
    }
}