namespace ZeroUnit.Example.Tests;

public class NonTests
{
    internal void Internal()
    {
        throw new InvalidOperationException("Not a test. Internal method");
    }

    private void Private()
    {
        throw new InvalidOperationException("Not a test. Private method");
    }

    public int ReturnValue()
    {
        throw new InvalidOperationException("Not a test. Returns a value");
    }

    public Task<int> ReturnValueAsync()
    {
        throw new InvalidOperationException("Not a test. Returns a value");
    }

    public void Parameter(int x)
    {
        throw new InvalidOperationException("Not a test. Has a parameter");
    }
}