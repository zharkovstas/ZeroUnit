namespace ZeroUnit.Example.Tests;

public class GenericNonTests<T>
{
    public void Test()
    {
        throw new InvalidOperationException("Not a test. Instance method inside a generic class");
    }
}