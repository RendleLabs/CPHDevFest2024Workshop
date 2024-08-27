using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace RandomBenchmarks;

public class ForeachBenchmark
{
    private static readonly List<int> Numbers = Enumerable.Range(0, 1000).ToList();

    [Benchmark(Baseline = true)]
    public long Foreach()
    {
        long result = 0;
        foreach (var number in Numbers)
        {
            result += number;
        }

        return result;
    }

    [Benchmark]
    public long ForeachSpan()
    {
        long result = 0;
        var span = CollectionsMarshal.AsSpan(Numbers);
        
        foreach (var number in span)
        {
            result += number;
        }

        return result;
    }
}