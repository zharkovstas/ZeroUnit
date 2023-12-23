namespace ZeroUnit.Example.Tests;

public class ProtectedConstructorNonTests
{
    protected ProtectedConstructorNonTests()
    {
    }

    public void Test()
    {
        throw new InvalidOperationException("Not a test. Inside a class with no public constructor.");
    }
}