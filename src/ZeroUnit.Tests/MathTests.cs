namespace ZeroUnit.Tests;

public static class StackTests
{
    public static void Count_GivenEmptyStack_ReturnsZero()
    {
        var stack = new Stack<int>();

        var actual = stack.Count;

        if (actual == 0) { }
        else throw new InvalidOperationException($"Expected 4, got {actual}");
    }

    public static void Plus_GivenSingleElement_ReturnsOne()
    {
        var stack = new Stack<int>();
        stack.Push(5);

        var actual = stack.Count;

        if (actual == 1) { }
        else throw new InvalidOperationException($"Expected 5, got {actual}");
    }
}