using System.Text;
using BenchmarkDotNet.Attributes;

namespace OneBRC.Benchmarks;

[MemoryDiagnoser]
public class LineBenchmarks
{
    private const string Line = "Copenhagen;25.098";
    private static readonly byte[] LineBytes = Encoding.UTF8.GetBytes(Line);
    
    [Benchmark(Baseline = true)]
    public float AsString()
    {
        var parts = Line.Split(';');
        var value = float.Parse(parts[1]);
        return value;
    }

    [Benchmark]
    public float AsBytes()
    {
        var span = LineBytes.AsSpan();
        int sc = span.IndexOf((byte)';');
        span = span.Slice(sc + 1);
        var value = float.Parse(span);
        return value;
    }
}