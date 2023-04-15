namespace ZeroUnit.Tests;

public sealed class StackTests : IDisposable
{
    private Stack<int> stack;

    public StackTests()
    {
        stack = new Stack<int>();
    }

    public void Count_GivenEmptyStack_ReturnsZero()
    {
        var actual = stack.Count;

        if (actual == 0) { }
        else throw new InvalidOperationException($"Expected 4, got {actual}");
    }

    public async Task Plus_GivenSingleElement_ReturnsOne()
    {
        await Task.Delay(500).ConfigureAwait(false);
        stack.Push(5);

        var actual = stack.Count;

        if (actual == 1) { }
        else throw new InvalidOperationException($"Expected 5, got {actual}");
    }

    public void Dispose()
    {
        stack.Clear();
    }
}
