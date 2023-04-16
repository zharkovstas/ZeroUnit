namespace ZeroUnit.Tests;

public abstract class AbstractNonTests
{
    public void Test()
    {
        throw new InvalidOperationException("Not a test. Instance method inside an abstract class");
    }
}