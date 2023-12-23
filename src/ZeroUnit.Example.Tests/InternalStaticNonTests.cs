namespace ZeroUnit.Example.Tests;

internal static class InternalStaticNonTests
{
    public static void Test()
    {
        throw new InvalidOperationException("Not a test. Inside an internal class");
    }
}