namespace ZeroUnit.Example.Tests;

public static class StaticTests
{
    public static void Synchronous()
    {
    }

    public static Task Asynchronous1()
    {
        return Task.CompletedTask;
    }
#pragma warning disable CS1998
    public static async Task Asynchronous2()
    {
    }
#pragma warning restore CS1998

    public static System.Threading.Tasks.Task Asynchronous3()
    {
        return Task.CompletedTask;
    }
}