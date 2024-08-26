using System.Runtime.InteropServices;
using System.Text;
using TrafficCamera.Shared;

namespace TrafficCamera;

public class IntegerKeyImpl
{
    private readonly string _filepath;

    public IntegerKeyImpl(string filepath)
    {
        _filepath = filepath;
    }

    public ValueTask<Dictionary<string, Accumulator>> Run()
    {
        var buffer = new byte[1024];

        using var stream = File.OpenRead(_filepath);

        var aggregate = new Dictionary<int, Accumulator>();
        
        var read = stream.Read(buffer, 0, buffer.Length);

        int outerLoop = 0;
        int linesRead = 0;
        
        Span<Range> ranges = stackalloc Range[5];

        while (read > 0)
        {
            start:
            Console.WriteLine($"Outer loop {++outerLoop}");
            var span = buffer.AsSpan(0, read);
                
            while (span.Length > 0)
            {
#if(DEBUG)
                var str = Encoding.UTF8.GetString(span);
#endif

                var newline = span.IndexOf((byte)'\n');

                if (newline == -1)
                {
                    span.CopyTo(buffer);
                    var readSpan = buffer.AsSpan(span.Length);
                    read = stream.Read(readSpan);
                    if (read == 0) goto done;
                    read += span.Length;
                    goto start;
                }

                ++linesRead;

                span.Slice(0, newline).Split(ranges, (byte)';');

                var road = span[ranges[1]];
                var licensePlate = span[ranges[2]];
                var speed = float.Parse(span[ranges[4]]);

                var key = Key(road);
                if (!aggregate.TryGetValue(key, out var accumulator))
                {
                    aggregate[key] = accumulator = new Accumulator(Encoding.UTF8.GetString(road));
                }

                accumulator.Record(speed, Encoding.UTF8.GetString(licensePlate));

                span = span.Slice(newline + 1);
            }

            read = stream.Read(buffer);
        }
        
        done:
        return new ValueTask<Dictionary<string, Accumulator>>(
            aggregate.Values.ToDictionary(a => a.Road));
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