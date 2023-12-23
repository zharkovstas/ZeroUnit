using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ZeroUnit;

internal static class RunTestsMethodGenerator
{
    internal static string Generate(
        IMethodSymbol? partialMethod,
        IMethodSymbol[] testMethods)
    {
        var sb = new CodeStringBuilder();
        Generate(sb, partialMethod, testMethods);
        return sb.ToString();
    }

    internal static void Generate(
        CodeStringBuilder sb,
        IMethodSymbol? partialMethod,
        IMethodSymbol[] testMethods)
    {
        GenerateRunTestsMethod(sb, partialMethod, testMethods);
        sb.AppendLine();
        GenerateRunSingleTestMethod(sb, testMethods);
    }

    internal static string GenerateStub(IMethodSymbol? partialMethod)
    {
        var sb = new CodeStringBuilder();
        GenerateStub(sb, partialMethod);
        return sb.ToString();
    }

    internal static void GenerateStub(CodeStringBuilder sb, IMethodSymbol? partialMethod)
    {
        GenerateRunTestsMethodHeader(sb, partialMethod, isStub: true);
        sb.AppendLine("{");
        sb.AppendUnindentedLine(@"#pragma warning disable CA1303");
        sb.AppendLine(@"    Console.WriteLine(""Failed: 0, Passed: 0, Total: 0"");");
        sb.AppendLine("    return Task.FromResult(0);");
        sb.AppendUnindentedLine(@"#pragma warning restore CA1303");
        sb.AppendLine("}");
    }

    private static void GenerateRunTestsMethod(
        CodeStringBuilder sb,
        IMethodSymbol? partialMethod,
        IMethodSymbol[] testMethods)
    {
        GenerateRunTestsMethodHeader(sb, partialMethod, isStub: false);
        sb.AppendLine("{");
        sb.Indent();

        sb.AppendLine("var passedCount = 0;");
        sb.AppendLine();

        sb.AppendLine("var testNames = new[]");
        sb.AppendLine("{");
        sb.Indent();
        foreach (var method in testMethods)
        {
            var methodFullName = $"{NameHelper.GetFullName(method.ContainingType)}.{method.Name}";
            sb.AppendLine(@$"""{methodFullName}"",");
        }
        sb.Unindent();
        sb.AppendLine("};");

        sb.AppendLine();
        sb.AppendLine("using var finishedTests = new BlockingCollection<(string Name, string FailMessage)>();");
        sb.AppendLine("var failedTests = new ConcurrentQueue<(string Name, Exception Exception)>();");
        sb.AppendLine("var printProgressTask = Task.Run(() =>");
        sb.AppendLine("{");
        sb.Indent();
        sb.AppendLine("foreach (var (name, failMessage) in finishedTests.GetConsumingEnumerable())");
        sb.AppendLine("{");
        sb.AppendLine("    if (failMessage != null)");
        sb.AppendLine("    {");
        sb.AppendLine("        Console.ForegroundColor = ConsoleColor.Red;");
        sb.AppendLine(@"        Console.WriteLine($""x {name}: {failMessage}"");");
        sb.AppendLine("        Console.ResetColor();");
        sb.AppendLine("    }");
        sb.AppendLine("    else");
        sb.AppendLine("    {");
        sb.AppendLine("        Console.ForegroundColor = ConsoleColor.Green;");
        sb.AppendLine(@"        Console.WriteLine($""v {name}"");");
        sb.AppendLine("        Console.ResetColor();");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        sb.Unindent();
        sb.AppendLine("});");
        sb.AppendLine();
        sb.AppendUnindentedLine("#pragma warning disable CA1031");
        sb.AppendLine("await Parallel");
        sb.AppendLine($"    .ForEachAsync(Enumerable.Range(0, {testMethods.Length}), async (i, cancellationToken) =>");
        sb.AppendLine("    {");
        sb.AppendLine("        try");
        sb.AppendLine("        {");
        sb.AppendLine("            await RunTestAsync(i).ConfigureAwait(false);");
        sb.AppendLine("            Interlocked.Increment(ref passedCount);");
        sb.AppendLine("            finishedTests.Add((testNames[i], null), cancellationToken);");
        sb.AppendLine("        }");
        sb.AppendLine("        catch (Exception ex)");
        sb.AppendLine("        {");
        sb.AppendLine("            finishedTests.Add((testNames[i], ex.Message), cancellationToken);");
        sb.AppendLine("            failedTests.Enqueue((testNames[i], ex));");
        sb.AppendLine("        }");
        sb.AppendLine("    })");
        sb.AppendLine("    .ConfigureAwait(false);");
        sb.AppendUnindentedLine("#pragma warning restore CA1031");
        sb.AppendLine();
        sb.AppendLine("finishedTests.CompleteAdding();");
        sb.AppendLine("await printProgressTask.ConfigureAwait(false);");
        sb.AppendLine("Console.WriteLine();");
        sb.AppendLine();
        sb.AppendLine("if (!failedTests.IsEmpty)");
        sb.AppendLine("{");
        sb.AppendLine("    Console.ForegroundColor = ConsoleColor.Red;");
        sb.AppendLine("    foreach (var (name, exception) in failedTests)");
        sb.AppendLine("    {");
        sb.AppendLine(@"        Console.WriteLine($""Failed test: {name}"");");
        sb.AppendLine(@"        Console.WriteLine();");
        sb.AppendLine("        Console.WriteLine(exception.Message);");
        sb.AppendLine(@"        Console.WriteLine();");
        sb.AppendLine(@"        Console.WriteLine($""Exception type: {exception.GetType()}"");");
        sb.AppendLine(@"        Console.WriteLine($""Stack trace: {exception.StackTrace}"");");
        sb.AppendLine(@"        Console.WriteLine();");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        sb.AppendLine("else");
        sb.AppendLine("{");
        sb.AppendLine("    Console.ForegroundColor = ConsoleColor.Green;");
        sb.AppendLine("}");
        sb.AppendLine();
        sb.AppendLine(@"Console.WriteLine($""Failed: {failedTests.Count}, Passed: {passedCount}, Total: {failedTests.Count + passedCount}"");");
        sb.AppendLine("Console.ResetColor();");
        sb.AppendLine();
        sb.AppendLine("return failedTests.Count;");
        sb.Unindent();
        sb.AppendLine("}");
    }

    private static void GenerateRunTestsMethodHeader(
        CodeStringBuilder sb,
        IMethodSymbol? partialMethod,
        bool isStub)
    {
        if (partialMethod is not null)
        {
            GeneratePartialRunTestsMethodHeader(sb, partialMethod, isStub);
        }
        else if (isStub)
        {
            sb.AppendLine("private static Task<int> RunTestsAsync()");
        }
        else
        {
            sb.AppendLine("private static async Task<int> RunTestsAsync()");
        }
    }

    private static void GeneratePartialRunTestsMethodHeader(
        CodeStringBuilder sb,
        IMethodSymbol partialMethod,
        bool isStub)
    {
        var modifiers = partialMethod.DeclaringSyntaxReferences
            .Select(x => x.GetSyntax())
            .OfType<MethodDeclarationSyntax>()
            .First(x => x.Body is null)
            .Modifiers
            .Select(x => x.ToString())
            .Where(x => x != "async")
            .ToList();

        if (!isStub)
        {
            modifiers.Add("async");
        }

        sb.AppendLine($"{string.Join(" ", modifiers)} Task<int> RunTestsAsync()");
    }

    private static void GenerateRunSingleTestMethod(
        CodeStringBuilder sb,
        IMethodSymbol[] testMethods)
    {
        sb.AppendUnindentedLine("#pragma warning disable CS1998");
        sb.AppendLine("private static async Task RunTestAsync(int index)");
        sb.AppendLine("{");
        sb.AppendLine("    switch (index)");
        sb.AppendLine("    {");
        sb.Indent();
        sb.Indent();
        for (int i = 0; i < testMethods.Length; i++)
        {
            var method = testMethods[i];
            sb.AppendLine($"case {i}:");
            sb.Indent();
            TestCallGenerator.Generate(sb, method, i);
            sb.AppendLine("break;");
            sb.Unindent();
        }
        sb.AppendLine("default:");
        sb.AppendLine("    break;");
        sb.Unindent();
        sb.Unindent();
        sb.AppendLine("    }");
        sb.AppendLine("}");
        sb.AppendUnindentedLine("#pragma warning restore CS1998");
    }
}