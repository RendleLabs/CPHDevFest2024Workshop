using System.Text;
using BenchmarkDotNet.Attributes;
using OneBRC.Shared;

namespace OneBRC.Benchmarks;

[MemoryDiagnoser]
public class KeyBenchmarks
{
    private const string Line = "Copenhagen;25.098";
    private static readonly byte[] LineBytes = Encoding.UTF8.GetBytes(Line);
    
    [Benchmark(Baseline = true)]
    public string AsString()
    {
        var span = LineBytes.AsSpan();
        int sc = span.IndexOf((byte)';');
        return Encoding.UTF8.GetString(span.Slice(0, sc));
    }

    [Benchmark]
    public CityKey AsKey()
    {
        var span = LineBytes.AsSpan();
        int sc = span.IndexOf((byte)';');
        return CityKey.FromSpan(span.Slice(0, sc));
    }
}