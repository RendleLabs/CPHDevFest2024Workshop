using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;
using TrafficCamera.Shared;

namespace TrafficCamera;

public class MemoryMappedFileImpl
{
    private readonly string _filepath;

    public MemoryMappedFileImpl(string filepath)
    {
        _filepath = filepath;
    }

    public unsafe ValueTask<Dictionary<string, Accumulator>> Run()
    {
        var size = new FileInfo(_filepath).Length;

        var mmf = MemoryMappedFile.CreateFromFile(_filepath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);

        var view = mmf.CreateViewAccessor(0, size, MemoryMappedFileAccess.Read);
        byte* ptr = null;
        view.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
        var span = new ReadOnlySpan<byte>(ptr, (int)size);

        return new ValueTask<Dictionary<string, Accumulator>>(Run(span));
    }

    private Dictionary<string, Accumulator> Run(ReadOnlySpan<byte> span)
    {
        var aggregate = new Dictionary<int, Accumulator>();
        Span<Range> ranges = stackalloc Range[5];
        while (span.Length > 0)
        {
            var newline = span.IndexOf((byte)'\n');

            if (newline == -1)
            {
                break;
            }

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

        return aggregate.Values.ToDictionary(a => a.Road);
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