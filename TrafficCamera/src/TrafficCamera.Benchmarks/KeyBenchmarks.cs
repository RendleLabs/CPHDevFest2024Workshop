using System.Runtime.InteropServices;
using System.Text;
using BenchmarkDotNet.Attributes;
using TrafficCamera.Shared;

namespace TrafficCamera.Benchmarks;

[MemoryDiagnoser]
public class KeyBenchmarks
{
    private const string Line = "2024-08-26T05:00:07.503;A236;YJ36 TLE;Blue;46.2";
    private const string Line2 = "2024-08-26T05:00:07.503;A23;YJ36 TLE;Blue;46.2";
    private static readonly byte[] LineBytes = Encoding.UTF8.GetBytes(Line);
    private static readonly byte[] Line2Bytes = Encoding.UTF8.GetBytes(Line2);

    [Benchmark(Baseline = true)]
    public string StringKey()
    {
        var span = LineBytes.AsSpan();
        Span<Range> ranges = stackalloc Range[5];
        span.Split(ranges, (byte)';');
        return Encoding.UTF8.GetString(span[ranges[1]]);
    }
    
    [Benchmark]
    public int Int32Key()
    {
        var span = LineBytes.AsSpan();
        Span<Range> ranges = stackalloc Range[5];
        span.Split(ranges, (byte)';');
        return Key(span[ranges[1]]);
    }
    
    private static int Key(ReadOnlySpan<byte> span)
    {
        if (span.Length == 4)
        {
            return MemoryMarshal.Read<int>(span);
        }
        Span<byte> four = stackalloc byte[4];
        span.CopyTo(four);
        return MemoryMarshal.Read<int>(four);
    }
}