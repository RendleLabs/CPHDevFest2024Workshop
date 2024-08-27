using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;

namespace TrafficCamera.Benchmarks;

public class SimdBenchmark
{
    private static readonly Vector256<int>[] Vectors = Enumerable.Range(0, 10000)
        .Select(Vector256.Create)
        .ToArray();
    
    private static readonly int[][] Arrays = Enumerable.Range(0, 10000)
        .Select(n => new[]{n,n,n,n,n,n,n,n})
        .ToArray();

    [Benchmark(Baseline = true)]
    public long Array()
    {
        int[] t = [0, 0, 0, 0, 0, 0, 0, 0];

        for (int i = 0; i < Arrays.Length; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                t[j] = Arrays[i][j];
            }
        }

        return t[0];
    }

    [Benchmark]
    public long VectorOperator()
    {
        var t = Vector256.Create(0);

        foreach (var vector in Vectors)
        {
            t += vector;
        }

        return t[0];
    }

    [Benchmark]
    public long VectorMethod()
    {
        var t = Vector256.Create(0);

        foreach (var vector in Vectors)
        {
            t = Vector256.Add(t, vector);
        }

        return t[0];
    }
}