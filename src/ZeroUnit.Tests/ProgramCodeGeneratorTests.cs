using Microsoft.CodeAnalysis;

using That;

namespace ZeroUnit.Tests;

public class ProgramCodeGeneratorTests
{
    public void GivenNoMainMethod_GeneratesInternalStaticProgramClass()
    {
        var actual = ProgramCodeGenerator.Generate(
            mainMethod: null,
            methods: Array.Empty<IMethodSymbol>());

        Assert.That(
            actual.Contains(
                "internal static class Program",
                StringComparison.InvariantCulture),
            actual);
    }

    public void GivenNoMainMethod_GeneratesMainMethod()
    {
        var actual = ProgramCodeGenerator.Generate(
            mainMethod: null,
            methods: Array.Empty<IMethodSymbol>());

        var expectedMethod = @"    public static int Main()
    {
        return RunTestsAsync().GetAwaiter().GetResult();
    }";

        Assert.That(
            actual.Contains(expectedMethod, StringComparison.InvariantCulture),
            actual);
    }

    public void GeneratesRunTestsMethod()
    {
        var actual = ProgramCodeGenerator.Generate(
            mainMethod: null,
            methods: Array.Empty<IMethodSymbol>());

        Assert.That(
            actual.Contains("Task<int> RunTestsAsync()", StringComparison.InvariantCulture),
            actual);
    }

    public void GivenNonPartialProgram_GeneratesEmptyString()
    {
        var code = @"class Program { static void Main() {} }";
        var mainMethod = CodeHelper.GetMethodSymbols(code).Single();

        var actual = ProgramCodeGenerator.Generate(
            mainMethod: mainMethod,
            methods: Array.Empty<IMethodSymbol>());

        Assert.That(actual.Length == 0, actual);
    }

    public void GivenProgram_DoesNotGenerateMainMethod()
    {
        var code = @"partial class Program { static void Main() {} }";
        var mainMethod = CodeHelper.GetMethodSymbols(code).Single();

        var actual = ProgramCodeGenerator.Generate(
            mainMethod: mainMethod,
            methods: Array.Empty<IMethodSymbol>());

        Assert.That(
            !actual.Contains("Main", StringComparison.InvariantCulture),
            actual);
    }

    public void GivenProgram_UsesSameClassName()
    {
        var code = @"partial class MyEntryPoint { static void Main() {} }";
        var mainMethod = CodeHelper.GetMethodSymbols(code).Single();

        var actual = ProgramCodeGenerator.Generate(
            mainMethod: mainMethod,
            methods: Array.Empty<IMethodSymbol>());

        Assert.That(
            actual.Contains("partial class MyEntryPoint", StringComparison.InvariantCulture),
            actual);
    }

    public void GivenImplicitlyInternalProgram_GeneratesInternalClass()
    {
        var code = @"partial class Program { static void Main() {} }";
        var mainMethod = CodeHelper.GetMethodSymbols(code).Single();

        var actual = ProgramCodeGenerator.Generate(
            mainMethod: mainMethod,
            methods: Array.Empty<IMethodSymbol>());

        Assert.That(
            actual.Contains("internal partial class Program", StringComparison.InvariantCulture),
            actual);
    }

    public void GivenInternalProgram_GeneratesInternalClass()
    {
        var code = @"internal partial class Program { static void Main() {} }";
        var mainMethod = CodeHelper.GetMethodSymbols(code).Single();

        var actual = ProgramCodeGenerator.Generate(
            mainMethod: mainMethod,
            methods: Array.Empty<IMethodSymbol>());

        Assert.That(
            actual.Contains("internal partial class Program", StringComparison.InvariantCulture),
            actual);
    }

    public void GivenPublicProgram_GeneratesPublicClass()
    {
        var code = @"public partial class Program { static void Main() {} }";
        var mainMethod = CodeHelper.GetMethodSymbols(code).Single();

        var actual = ProgramCodeGenerator.Generate(
            mainMethod: mainMethod,
            methods: Array.Empty<IMethodSymbol>());

        Assert.That(
            actual.Contains("public partial class Program", StringComparison.InvariantCulture),
            actual);
    }

    public void GivenStaticProgram_GeneratesStaticClass()
    {
        var code = @"static partial class Program { static void Main() {} }";
        var mainMethod = CodeHelper.GetMethodSymbols(code).Single();

        var actual = ProgramCodeGenerator.Generate(
            mainMethod: mainMethod,
            methods: Array.Empty<IMethodSymbol>());

        Assert.That(
            actual.Contains("static partial class Program", StringComparison.InvariantCulture),
            actual);
    }

    public void GivenNamespace_UsesSameNamespace()
    {
        var code = @"namespace My.Tests { partial class Program { static void Main() {} } }";
        var mainMethod = CodeHelper.GetMethodSymbols(code).Single();

        var actual = ProgramCodeGenerator.Generate(
            mainMethod: mainMethod,
            methods: Array.Empty<IMethodSymbol>());

        Assert.That(
            actual.Contains("namespace My.Tests", StringComparison.InvariantCulture),
            actual);
    }
}