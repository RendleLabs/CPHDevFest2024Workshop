using System.IO.MemoryMappedFiles;
using System.Text;
using OneBRC.Shared;

namespace OneBRC;

public class ThreadedImpl
{
    private readonly string _filePath;

    public ThreadedImpl(string filePath)
    {
        _filePath = filePath;
    }

    public ValueTask<Dictionary<string, Accumulator>> Run()
    {
        var size = new FileInfo(_filePath).Length;
        var mmf = MemoryMappedFile.CreateFromFile(_filePath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
        var view = mmf.CreateViewAccessor(0, size, MemoryMappedFileAccess.Read);

        var chunks = mmf.GetChunks(size, Environment.ProcessorCount);

        var parsers = chunks
            .Select(c => new ChunkParser(view, c))
            .ToArray();

        var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };

        Parallel.ForEach(parsers, options, p => p.Run());

        var result = new Dictionary<string, Accumulator>();

        foreach (var parser in parsers)
        {
            foreach (var acc in parser.Aggregate.Values)
            {
                if (result.TryGetValue(acc.City, out var accumulator))
                {
                    accumulator.Combine(acc);
                }
                else
                {
                    result[acc.City] = acc;
                }
            }
        }

        return new ValueTask<Dictionary<string, Accumulator>>(result);
    }

    private class ChunkParser
    {
        private readonly MemoryMappedViewAccessor _view;
        private readonly MemoryMappedFileChunk _chunk;

        public ChunkParser(MemoryMappedViewAccessor view, MemoryMappedFileChunk chunk)
        {
            _view = view;
            _chunk = chunk;
        }

        public Dictionary<CityKey, Accumulator> Aggregate { get; } = new();

        public unsafe void Run()
        {
            var chunkLength = _chunk.Length;

            byte* ptr = null;
            _view.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
            var span = new ReadOnlySpan<byte>(ptr + _chunk.Offset, chunkLength);
            
            #if(DEBUG)
            var str = Encoding.UTF8.GetString(span[^16..]);
            #endif

            while (span.Length > 0)
            {
                var endOfLine = span.IndexOf((byte)'\n');

                if (endOfLine < 0)
                {
                    break;
                }

                var line = span.Slice(0, endOfLine);
                ProcessLine(line, Aggregate);
                span = span.Slice(endOfLine + 1);
            }
        }

        private static void ProcessLine(ReadOnlySpan<byte> line, Dictionary<CityKey, Accumulator> aggregate)
        {
            var semicolon = line.IndexOf((byte)';');
            var name = line.Slice(0, semicolon);
            var temp = line.Slice(semicolon + 1);

            var cityKey = CityKey.FromSpan(name);
            var value = IntParser.Parse(temp);

            if (!aggregate.TryGetValue(cityKey, out var accumulator))
            {
                accumulator = aggregate[cityKey] = new Accumulator(Encoding.UTF8.GetString(name));
            }

            accumulator.Record(value);
        }
    }
}