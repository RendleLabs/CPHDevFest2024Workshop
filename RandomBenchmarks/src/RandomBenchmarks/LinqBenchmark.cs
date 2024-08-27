using BenchmarkDotNet.Attributes;

namespace RandomBenchmarks;

public class LinqBenchmark
{
    private static readonly string[] Strings = ["Foo", "Bar", "Quux", "Wibble"];

    [Benchmark(Baseline = true)]
    public int UsingLinq()
    {
        return Strings.Select(s => s.Length).Max();
    }

    [Benchmark]
    public int UsingLoop()
    {
        int max = 0;
        
        for (int i = 0, l = Strings.Length; i < l; i++)
        {
            max = Math.Max(max, Strings[i].Length);
        }

        return max;
    }
}