using System.Text;
using BenchmarkDotNet.Attributes;
using TrafficCamera.Shared;

namespace TrafficCamera.Benchmarks;

public class RangeBenchmarks
{
    private const string Line = "2024-08-26T05:00:07.503;A236;YJ36 TLE;Blue;46.2";
    private static readonly byte[] LineBytes = Encoding.UTF8.GetBytes(Line);

    [Benchmark(Baseline = true)]
    public float Indexer()
    {
        var span = LineBytes.AsSpan();
        Span<Range> ranges = stackalloc Range[5];
        span.Split(ranges, (byte)';');
        return float.Parse(span[ranges[4]]);
    }
    
    [Benchmark]
    public float Slice()
    {
        var span = LineBytes.AsSpan();
        Span<Range> ranges = stackalloc Range[5];
        span.Split(ranges, (byte)';');
        var start = ranges[4].Start.Value;
        var end = ranges[4].End.Value - start;
        return float.Parse(span.Slice(start, end));
    }
}