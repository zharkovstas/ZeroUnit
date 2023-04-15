using System.Text;

using Microsoft.CodeAnalysis;

namespace ZeroUnit;

internal static class ProgramCodeGenerator
{
    internal static string Generate(IReadOnlyList<IMethodSymbol> methods)
    {
        var callableMethods = methods
            .Where(x => x.IsStatic || x.ContainingType.InstanceConstructors.Any(x => x.Parameters.Length == 0))
            .Where(x => x.Name != "Dispose")
            .ToArray();

        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Concurrent;");
        sb.AppendLine("using System.Threading;");
        sb.AppendLine("using System.Threading.Tasks;");
        sb.AppendLine();
        sb.AppendLine("namespace Tests");
        sb.AppendLine("{");
        sb.AppendLine("    internal sealed class Program");
        sb.AppendLine("    {");
        if (callableMethods.Length > 0)
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
        if (callableMethods.Length > 0)
        {

            sb.AppendLine();
            sb.AppendLine("            var testNames = new[]");
            sb.AppendLine("            {");
            foreach (var method in callableMethods)
            {
                var methodFullName = $"{GetFullMetadataName(method.ContainingType)}.{method.Name}";
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
            sb.AppendLine($"                .ForEachAsync(Enumerable.Range(0, {callableMethods.Length}), async (i, cancellationToken) =>");
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
        if (callableMethods.Length > 0)
        {

            sb.AppendLine();
            sb.AppendLine("#pragma warning disable CS1998");
            sb.AppendLine("        private static async Task RunTestAsync(int index)");
            sb.AppendLine("        {");
            sb.AppendLine("            switch (index)");
            sb.AppendLine("            {");
            for (int i = 0; i < callableMethods.Length; i++)
            {
                var method = callableMethods[i];
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
        var containingTypeFullName = GetFullMetadataName(containingType);
        var isContaningTypeDisposable = containingType.AllInterfaces.Any(x => GetFullMetadataName(x) == "System.IDisposable");

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

        if (method.IsAsync)
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

        if (method.IsAsync)
        {
            sb.Append(".ConfigureAwait(false)");
        }

        sb.Append(";");

        return sb.ToString();
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