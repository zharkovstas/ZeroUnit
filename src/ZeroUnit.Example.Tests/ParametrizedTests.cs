namespace ZeroUnit.Example.Tests;

public class ParametrizedTests
{
    public void ParallelTest()
    {
        Parallel.ForEach(
            new (int First, int Second, int Expected)[]
            {
                (1, 1, 2),
                (3, 4, 7)
            },
            x => TestSum(x.First, x.Second, x.Expected));
    }

    public void SequentialTest()
    {
        var testCases = new (int First, int Second, int Expected)[]
        {
            (1, 1, 2),
            (3, 4, 7)
        };

        var exceptions = new List<Exception>();

        foreach (var (first, second, expected) in testCases)
        {
            try
            {
                TestSum(first, second, expected);
            }
            catch (InvalidOperationException e)
            {
                exceptions.Add(e);
            }
        }

        if (exceptions.Any()) throw new AggregateException(exceptions);
    }

    private void TestSum(int first, int second, int expected)
    {
        var actual = first + second;
        if (actual != expected)
        {
            throw new InvalidOperationException($"Expected {first} + {second} to equal {expected}, got {actual}");
        }
    }
}