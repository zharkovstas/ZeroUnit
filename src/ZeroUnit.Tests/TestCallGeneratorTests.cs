using That;

namespace ZeroUnit.Tests;

public class TestCallGeneratorTests
{
    public void GivenInstanceTest_GeneratesCall()
    {
        var code = @"class Tests { void Test() {} }";
        var method = CodeHelper.GetMethodSymbols(code).Single();

        var expected = @"var fixture0 = new Tests();
fixture0.Test();
";

        var actual = TestCallGenerator.Generate(method, 0);

        Assert.That(actual == expected, actual);
    }

    public void AddsFixtureIndex()
    {
        var code = @"class Tests { void Test() {} }";
        var method = CodeHelper.GetMethodSymbols(code).Single();

        var expected = @"var fixture7 = new Tests();
fixture7.Test();
";

        var actual = TestCallGenerator.Generate(method, 7);

        Assert.That(actual == expected, actual);
    }

    public void GivenStaticTest_GeneratesStaticCall()
    {
        var code = @"class Tests { static void Test() {} }";
        var method = CodeHelper.GetMethodSymbols(code).Single();

        var expected = @"Tests.Test();
";

        var actual = TestCallGenerator.Generate(method, 0);

        Assert.That(actual == expected, actual);
    }

    public void GivenAsyncTest_GeneratesAsyncCall()
    {
        var code = @"class Tests { Task Test() { return Task.CompletedTask; } }";
        var method = CodeHelper.GetMethodSymbols(code).Single();

        var expected = @"var fixture0 = new Tests();
await fixture0.Test().ConfigureAwait(false);
";

        var actual = TestCallGenerator.Generate(method, 0);

        Assert.That(actual == expected, actual);
    }

    public void GivenDisposableClass_GeneratesUsing()
    {
        var code = @"class Tests : System.IDisposable { void Test() {} }";
        var method = CodeHelper.GetMethodSymbols(code).Single();

        var expected = @"using (var fixture0 = new Tests())
{
    fixture0.Test();
}
";

        var actual = TestCallGenerator.Generate(method, 0);

        Assert.That(actual == expected, actual);
    }
}