using BenchmarkDotNet.Attributes;

namespace RandomBenchmarks;

public class SyncAsyncBenchmark
{
    private static readonly int[] Values = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
    
    [Benchmark(Baseline = true)]
    public int Sync()
    {
        int t = 0;
        
        foreach (var i in Values)
        {
            t += i;
        }

        return t;
    }
    
    [Benchmark]
    public Task<int> Async()
    {
        int t = 0;
        
        foreach (var i in Values)
        {
            t += i;
        }

        return Task.FromResult(t);
    }

    [Benchmark]
    public int ValueAsync()
    {
        var t = ValueAsyncImpl();
        if (t.IsCompleted) return t.Result;
        return t.GetAwaiter().GetResult();
    }
    
    private ValueTask<int> ValueAsyncImpl()
    {
        int t = 0;
        
        foreach (var i in Values)
        {
            t += i;
        }

        return new ValueTask<int>(t);
    }
}