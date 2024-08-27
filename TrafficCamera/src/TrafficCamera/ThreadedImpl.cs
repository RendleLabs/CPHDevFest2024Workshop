using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;
using TrafficCamera.Shared;

namespace TrafficCamera;

public class ThreadedImpl
{
    private readonly string _filepath;

    public ThreadedImpl(string filepath)
    {
        _filepath = filepath;
    }

    public ValueTask<Dictionary<string, IntAccumulator>> Run()
    {
        var size = new FileInfo(_filepath).Length;

        var mmf = MemoryMappedFile.CreateFromFile(_filepath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);

        var chunks = mmf.GetChunks(size, Environment.ProcessorCount, (byte)'\n');

        var view = mmf.CreateViewAccessor(0, size, MemoryMappedFileAccess.Read);

        var parsers = new ChunkParser[Environment.ProcessorCount];

        for (int i = 0; i < Environment.ProcessorCount; i++)
        {
            parsers[i] = new ChunkParser(view, chunks[i]);
        }

        var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };

        Parallel.ForEach(parsers, options, p => p.Run());

        var result = new Dictionary<string, IntAccumulator>();
        foreach (var parser in parsers)
        {
            foreach (var accumulator in parser.Accumulators)
            {
                if (result.TryGetValue(accumulator.Road, out var existing))
                {
                    existing.Combine(accumulator);
                }
                else
                {
                    result.Add(accumulator.Road, accumulator);
                }
            }
        }

        return new ValueTask<Dictionary<string, IntAccumulator>>(result);
    }

    private class ChunkParser
    {
        private readonly MemoryMappedViewAccessor _view;
        private readonly MemoryMappedFileChunk _chunk;
        private readonly Dictionary<int, IntAccumulator> _accumulators = new();

        public ChunkParser(MemoryMappedViewAccessor view, MemoryMappedFileChunk chunk)
        {
            _view = view;
            _chunk = chunk;
        }
        
        public IEnumerable<IntAccumulator> Accumulators => _accumulators.Values;

        public unsafe void Run()
        {
            byte* ptr = null;
            _view.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);

            ptr += _chunk.Offset;
            var span = new ReadOnlySpan<byte>(ptr, _chunk.Length);
            
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
                var color = span[ranges[3]];
                var speed = IntParser.Parse(span[ranges[4]]);

                var key = Key(road);
                if (!_accumulators.TryGetValue(key, out var accumulator))
                {
                    _accumulators[key] = accumulator = new IntAccumulator(Encoding.UTF8.GetString(road));
                }

                accumulator.Record(speed, licensePlate, Colors.Parse(color));

                span = span.Slice(newline + 1);
            }
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
}