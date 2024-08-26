using System.Text;
using OneBRC.Shared;

namespace OneBRC;

public class StreamImpl
{
    private readonly string _filepath;

    public StreamImpl(string filepath)
    {
        _filepath = filepath;
    }

    public ValueTask<Dictionary<string, Accumulator>> Run()
    {
        var aggregate = new Dictionary<string, Accumulator>();
        
        using var stream = File.Open(_filepath, FileMode.Open, FileAccess.Read, FileShare.Read);

        var buffer = new byte[1024];

        int read = stream.Read(buffer, 0, buffer.Length);

        while (read > 0)
        {
            var span = buffer.AsSpan(0, read);

            while (span.Length > 0)
            {
                var endOfLine = span.IndexOf((byte)'\n');

                if (endOfLine < 0)
                {
                    span.CopyTo(buffer);
                    var readSpan = buffer.AsSpan(span.Length);
                    read = stream.Read(readSpan);
                    if (read == 0)
                    {
                        goto done;
                    }

                    read += span.Length;
                    break;
                }
                
                var line = span.Slice(0, endOfLine);
                ProcessLine(line, aggregate);
                span = span.Slice(endOfLine + 1);
            }
        }
        
        done:
        return new ValueTask<Dictionary<string, Accumulator>>(aggregate);
    }

    private static void ProcessLine(ReadOnlySpan<byte> line, Dictionary<string, Accumulator> aggregate)
    {
        var semicolon = line.IndexOf((byte)';');
        var name = line.Slice(0, semicolon);
        var temp = line.Slice(semicolon + 1);

        var city = Encoding.UTF8.GetString(name);
        var value = float.Parse(temp);

        if (!aggregate.TryGetValue(city, out var accumulator))
        {
            accumulator = aggregate[city] = new Accumulator(city);
        }
        
        accumulator.Record(value);
    }
}