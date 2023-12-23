using That;

namespace ZeroUnit.Tests;

public class RunTestsMethodGeneratorTests
{
    public void Generate_GivenNoPartialMethod_GeneratesNonPartialMethod()
    {
        var testMethods = CodeHelper.GetMethodSymbols("class Tests { void Test() {} }");

        var actual = RunTestsMethodGenerator.Generate(
            partialMethod: null,
            testMethods);

        Assert.That(
            actual.StartsWith(
                "private static async Task<int> RunTestsAsync()",
                StringComparison.InvariantCulture),
            actual);
    }

    public void Generate_GeneratesRunSingleTestMethod()
    {
        var testMethods = CodeHelper.GetMethodSymbols("class Tests { void Test() {} }");

        var actual = RunTestsMethodGenerator.Generate(
            partialMethod: null,
            testMethods);

        Assert.That(
            actual.Contains(
                "private static async Task RunTestAsync(int index)",
                StringComparison.InvariantCulture),
            actual);
    }

    public void Generate_GivenPartialMethod_GeneratesPartialMethod()
    {
        var code = @"partial class Program {
    static void Main() {}
    private static partial Task<int> RunTestsAsync();
}";
        var partialMethod = CodeHelper.GetMethodSymbols(code).Single(x => x.Name == "RunTestsAsync");

        var testMethods = CodeHelper.GetMethodSymbols("class Tests { void Test() {} }");

        var actual = RunTestsMethodGenerator.Generate(
            partialMethod,
            testMethods);

        Assert.That(
            actual.StartsWith(
                "private static partial async Task<int> RunTestsAsync()",
                StringComparison.InvariantCulture),
            actual);
    }

    public void Generate_GivenInstanceMethod_GeneratesInstancePartialMethod()
    {
        var code = @"partial class Program {
    static void Main() {}
    private partial Task<int> RunTestsAsync();
}";
        var partialMethod = CodeHelper.GetMethodSymbols(code).Single(x => x.Name == "RunTestsAsync");

        var testMethods = CodeHelper.GetMethodSymbols("class Tests { void Test() {} }");

        var actual = RunTestsMethodGenerator.Generate(
            partialMethod,
            testMethods);

        Assert.That(
            actual.StartsWith(
                "private partial async Task<int> RunTestsAsync()",
                StringComparison.InvariantCulture),
            actual);
    }

    public void Generate_GivenPublicPartialMethod_GeneratesPublicPartialMethod()
    {
        var code = @"partial class Program {
    static void Main() {}
    public static partial Task<int> RunTestsAsync();
}";
        var partialMethod = CodeHelper.GetMethodSymbols(code).Single(x => x.Name == "RunTestsAsync");

        var testMethods = CodeHelper.GetMethodSymbols("class Tests { void Test() {} }");

        var actual = RunTestsMethodGenerator.Generate(
            partialMethod,
            testMethods);

        Assert.That(
            actual.StartsWith(
                "public static partial async Task<int> RunTestsAsync()",
                StringComparison.InvariantCulture),
            actual);
    }

    public void Generate_GivenProtectedInternalPartialMethod_GeneratesProtectedInternalPartialMethod()
    {
        var code = @"partial class Program {
    static void Main() {}
    protected internal static partial Task<int> RunTestsAsync();
}";
        var partialMethod = CodeHelper.GetMethodSymbols(code).Single(x => x.Name == "RunTestsAsync");

        var testMethods = CodeHelper.GetMethodSymbols("class Tests { void Test() {} }");

        var actual = RunTestsMethodGenerator.Generate(
            partialMethod,
            testMethods);

        Assert.That(
            actual.StartsWith(
                "protected internal static partial async Task<int> RunTestsAsync()",
                StringComparison.InvariantCulture),
            actual);
    }

    public void GenerateStub_GeneratesStub()
    {
        var actual = RunTestsMethodGenerator.GenerateStub(partialMethod: null);

        var expected = @"private static Task<int> RunTestsAsync()
{
#pragma warning disable CA1303
    Console.WriteLine(""Failed: 0, Passed: 0, Total: 0"");
    return Task.FromResult(0);
#pragma warning restore CA1303
}
";

        Assert.That(actual == expected, actual);
    }

    public void GenerateStub_GivenPartialMethod_GeneratesPartialMethod()
    {
        var code = @"partial class Program {
    static void Main() {}
    private static partial Task<int> RunTestsAsync();
}";
        var partialMethod = CodeHelper.GetMethodSymbols(code).Single(x => x.Name == "RunTestsAsync");

        var actual = RunTestsMethodGenerator.GenerateStub(partialMethod);

        Assert.That(
            actual.StartsWith(
                "private static partial Task<int> RunTestsAsync()",
                StringComparison.InvariantCulture),
            actual);
    }

    public void GenerateStub_GivenInstanceMethod_GeneratesInstancePartialMethod()
    {
        var code = @"partial class Program {
    static void Main() {}
    private partial Task<int> RunTestsAsync();
}";
        var partialMethod = CodeHelper.GetMethodSymbols(code).Single(x => x.Name == "RunTestsAsync");

        var actual = RunTestsMethodGenerator.GenerateStub(partialMethod);

        Assert.That(
            actual.StartsWith(
                "private partial Task<int> RunTestsAsync()",
                StringComparison.InvariantCulture),
            actual);
    }

    public void GenerateStub_GivenPublicPartialMethod_GeneratesPublicPartialMethod()
    {
        var code = @"partial class Program {
    static void Main() {}
    public static partial Task<int> RunTestsAsync();
}";
        var partialMethod = CodeHelper.GetMethodSymbols(code).Single(x => x.Name == "RunTestsAsync");

        var actual = RunTestsMethodGenerator.GenerateStub(partialMethod);

        Assert.That(
            actual.StartsWith(
                "public static partial Task<int> RunTestsAsync()",
                StringComparison.InvariantCulture),
            actual);
    }

    public void GenerateStub_GivenProtectedInternalPartialMethod_GeneratesProtectedInternalPartialMethod()
    {
        var code = @"partial class Program {
    static void Main() {}
    protected internal static partial Task<int> RunTestsAsync();
}";
        var partialMethod = CodeHelper.GetMethodSymbols(code).Single(x => x.Name == "RunTestsAsync");

        var actual = RunTestsMethodGenerator.GenerateStub(partialMethod);

        Assert.That(
            actual.StartsWith(
                "protected internal static partial Task<int> RunTestsAsync()",
                StringComparison.InvariantCulture),
            actual);
    }

    public void GenerateStub_GivenNoPartialMethod_GeneratesNonPartialMethod()
    {
        var actual = RunTestsMethodGenerator.GenerateStub(partialMethod: null);

        Assert.That(
            actual.StartsWith(
                "private static Task<int> RunTestsAsync()",
                StringComparison.InvariantCulture),
            actual);
    }
}