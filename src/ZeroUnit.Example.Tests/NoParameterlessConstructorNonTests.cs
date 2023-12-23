namespace ZeroUnit.Example.Tests;

public class NoParameterlessConstructorNonTests
{
    public NoParameterlessConstructorNonTests(int parameter)
    {
    }

    public void Test()
    {
        throw new InvalidOperationException("Not a test. Inside a class with no parameterless constructor.");
    }
}