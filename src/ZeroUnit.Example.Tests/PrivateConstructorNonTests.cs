namespace ZeroUnit.Example.Tests;

public class PrivateConstructorNonTests
{
    private PrivateConstructorNonTests()
    {
    }

    public void Test()
    {
        throw new InvalidOperationException("Not a test. Inside a class with no public constructor.");
    }
}