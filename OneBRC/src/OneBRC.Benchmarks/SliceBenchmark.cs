using BenchmarkDotNet.Attributes;

namespace OneBRC.Benchmarks;

[DisassemblyDiagnoser]
public class SliceBenchmark
{
    private static readonly ReadOnlyMemory<byte> Line = "Copenhagen;25.098"u8.ToArray();
    
    [Benchmark(Baseline = true)]
    public int Slice()
    {
        var span = Line.Span;
        var sc = span.IndexOf((byte)';');
        span = span.Slice(sc + 1);
        return span.Length;
    }
    
    [Benchmark]
    public int Range()
    {
        var span = Line.Span;
        var sc = span.IndexOf((byte)';');
        span = span[(sc + 1)..];
        return span.Length;
    }
}