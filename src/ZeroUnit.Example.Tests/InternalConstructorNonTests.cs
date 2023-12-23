namespace ZeroUnit.Example.Tests;

public class InternalConstructorNonTests
{
    internal InternalConstructorNonTests()
    {
    }

    public void Test()
    {
        throw new InvalidOperationException("Not a test. Inside a class with no public constructor.");
    }
}