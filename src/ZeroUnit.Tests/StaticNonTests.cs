namespace ZeroUnit.Tests;

public static class StaticNonTests
{
    internal static void Internal()
    {
        throw new InvalidOperationException("Not a test. Internal method");
    }

    private static void Private()
    {
        throw new InvalidOperationException("Not a test. Private method");
    }

    public static int ReturnValue()
    {
        throw new InvalidOperationException("Not a test. Returns a value");
    }

    public static Task<int> ReturnValueAsync()
    {
        throw new InvalidOperationException("Not a test. Returns a value");
    }

    public static void Parameter(int x)
    {
        throw new InvalidOperationException("Not a test. Has a parameter");
    }

    public static void Generic<T>()
    {
        throw new InvalidOperationException("Not a test. Is generic");
    }
}