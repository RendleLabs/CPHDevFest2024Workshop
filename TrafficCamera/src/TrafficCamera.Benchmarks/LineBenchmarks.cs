using System.Text;
using BenchmarkDotNet.Attributes;

namespace TrafficCamera.Benchmarks;

[MemoryDiagnoser]
public class LineBenchmarks
{
    private const string Line = "2024-08-26T05:00:07.503;A236;YJ36 TLE;Blue;46.2";
    private static readonly byte[] LineBytes = Encoding.UTF8.GetBytes(Line);
    
    [Benchmark(Baseline = true)]
    public float Strings()
    {
        var parts = Line.Split(';');
        var speed = float.Parse(parts[4]);
        return speed;
    }

    [Benchmark]
    public float Bytes()
    {
        var span = LineBytes.AsSpan();
        var lastSc = span.LastIndexOf((byte)';');
        span = span.Slice(lastSc + 1);
        var speed = float.Parse(span);
        return speed;
    }
}