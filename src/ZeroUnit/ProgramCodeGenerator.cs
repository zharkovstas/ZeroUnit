using System.Text;

using Microsoft.CodeAnalysis;

namespace ZeroUnit;

internal static class ProgramCodeGenerator
{
    internal static string Generate(IReadOnlyList<IMethodSymbol> methods)
    {
        var testMethods = methods
            .Where(Predicates.IsTestMethod)
            .ToArray();

        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Concurrent;");
        sb.AppendLine("using System.Threading;");
        sb.AppendLine("using System.Threading.Tasks;");
        sb.AppendLine();
        sb.AppendLine("namespace ZeroUnit");
        sb.AppendLine("{");
        sb.AppendLine("    internal sealed class Program");
        sb.AppendLine("    {");
        if (testMethods.Length > 0)
        {
            sb.AppendLine("        private static async Task<int> Main(string[] args)");
        }
        else
        {
            sb.AppendLine("        private static int Main(string[] args)");
        }
        sb.AppendLine("        {");
        sb.AppendLine("            var passedCount = 0;");
        sb.AppendLine("            var failedCount = 0;");
        if (testMethods.Length > 0)
        {

            sb.AppendLine();
            sb.AppendLine("            var testNames = new[]");
            sb.AppendLine("            {");
            foreach (var method in testMethods)
            {
                var methodFullName = $"{GetFullName(method.ContainingType)}.{method.Name}";
                sb.AppendLine(@$"                ""{methodFullName}"",");
            }
            sb.AppendLine("            };");
            sb.AppendLine();
            sb.AppendLine("            using var output = new BlockingCollection<string>();");
            sb.AppendLine("            var outputTask = Task.Run(() =>");
            sb.AppendLine("            {");
            sb.AppendLine("                foreach (var text in output.GetConsumingEnumerable())");
            sb.AppendLine("                {");
            sb.AppendLine("                    Console.WriteLine(text);");
            sb.AppendLine("                }");
            sb.AppendLine("            });");
            sb.AppendLine();
            sb.AppendLine("#pragma warning disable CA1031");
            sb.AppendLine("            await Parallel");
            sb.AppendLine($"                .ForEachAsync(Enumerable.Range(0, {testMethods.Length}), async (i, cancellationToken) =>");
            sb.AppendLine("                {");
            sb.AppendLine("                    try");
            sb.AppendLine("                    {");
            sb.AppendLine($"                        await RunTestAsync(i).ConfigureAwait(false);");
            sb.AppendLine("                        Interlocked.Increment(ref passedCount);");
            sb.AppendLine(@"                        output.Add($""v {testNames[i]}"", cancellationToken);");
            sb.AppendLine("                    }");
            sb.AppendLine("                    catch (Exception ex)");
            sb.AppendLine("                    {");
            sb.AppendLine("                        Interlocked.Increment(ref failedCount);");
            sb.AppendLine(@"                        output.Add($""x {testNames[i]}: {ex}"", cancellationToken);");
            sb.AppendLine("                    }");
            sb.AppendLine("                })");
            sb.AppendLine("                .ConfigureAwait(false);");
            sb.AppendLine("#pragma warning restore CA1031");
            sb.AppendLine();
            sb.AppendLine("            output.CompleteAdding();");
            sb.AppendLine("            await outputTask.ConfigureAwait(false);");
        }
        sb.AppendLine();
        sb.AppendLine("            Console.ForegroundColor = failedCount > 0 ? ConsoleColor.Red : ConsoleColor.Green;");
        sb.AppendLine(@"            Console.WriteLine($""Failed: {failedCount}, Passed: {passedCount}, Total: {failedCount + passedCount}"");");
        sb.AppendLine("            Console.ResetColor();");
        sb.AppendLine();
        sb.AppendLine("            return failedCount;");
        sb.AppendLine("        }");
        if (testMethods.Length > 0)
        {

            sb.AppendLine();
            sb.AppendLine("#pragma warning disable CS1998");
            sb.AppendLine("        private static async Task RunTestAsync(int index)");
            sb.AppendLine("        {");
            sb.AppendLine("            switch (index)");
            sb.AppendLine("            {");
            for (int i = 0; i < testMethods.Length; i++)
            {
                var method = testMethods[i];
                sb.AppendLine($"                case {i}:");
                sb.AppendLine($"{GetCallStatements(method, i, "                    ")}");
                sb.AppendLine("                    break;");
            }
            sb.AppendLine("                default:");
            sb.AppendLine("                    break;");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine("#pragma warning restore CS1998");
        }
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string GetCallStatements(IMethodSymbol method, int index, string prefix)
    {
        var containingType = method.ContainingType;
        var containingTypeFullName = GetFullName(containingType);
        var isContaningTypeDisposable = containingType.AllInterfaces.Any(x => GetFullName(x) == "System.IDisposable");

        var sb = new StringBuilder();

        if (!method.IsStatic)
        {
            sb.Append(prefix);
            if (isContaningTypeDisposable)
            {
                sb.Append("using (");
            }
            sb.Append($"var fixture{index} = new {containingTypeFullName}()");
            if (isContaningTypeDisposable)
            {
                sb.AppendLine(")");
            }
            else
            {
                sb.AppendLine(";");
            }
        }

        sb.Append(prefix);

        if (isContaningTypeDisposable)
        {
            sb.Append("    ");
        }

        if (!method.ReturnsVoid)
        {
            sb.Append("await ");
        }

        if (method.IsStatic)
        {
            sb.Append(containingTypeFullName);
        }
        else
        {
            sb.Append($"fixture{index}");
        }

        sb.Append($".{method.Name}()");

        if (!method.ReturnsVoid)
        {
            sb.Append(".ConfigureAwait(false)");
        }

        sb.Append(";");

        return sb.ToString();
    }

    private static string GetFullName(INamespaceOrTypeSymbol symbol)
    {
        ISymbol current = symbol;
        var parts = new List<string>
        {
            symbol.MetadataName
        };
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

        return string.Join("", parts);
    }
}