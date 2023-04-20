namespace ZeroUnit.Tests;

public sealed class DisposableNonTests : IDisposable
{
    public void Dispose()
    {
#pragma warning disable CA1065
        throw new InvalidOperationException($"Not a test. No test methods");
#pragma warning restore CA1065
    }
}