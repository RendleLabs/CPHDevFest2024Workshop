using System.Text;
using TrafficCamera.Shared;

namespace TrafficCamera;

public class StreamImpl
{
    private readonly string _filepath;

    public StreamImpl(string filepath)
    {
        _filepath = filepath;
    }

    public ValueTask<Dictionary<string, Accumulator>> Run()
    {
        var buffer = new byte[1024];

        using var stream = File.OpenRead(_filepath);

        var aggregate = new Dictionary<string, Accumulator>();
        
        var read = stream.Read(buffer, 0, buffer.Length);

        int outerLoop = 0;
        int linesRead = 0;

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

                int sc = span.IndexOf((byte)';');
                var time = span.Slice(0, sc);
                span = span.Slice(sc + 1);
                sc = span.IndexOf((byte)';');
                var road = span.Slice(0, sc);
                span = span.Slice(sc + 1);
                sc = span.IndexOf((byte)';');
                var licensePlate = span.Slice(0, sc);
                span = span.Slice(sc + 1);
                sc = span.IndexOf((byte)';');
                var color = span.Slice(0, sc);
                span = span.Slice(sc + 1);
                newline = span.IndexOf((byte)'\n');
                var speed = float.Parse(span.Slice(0, newline));

                var key = Encoding.UTF8.GetString(road);
                if (!aggregate.TryGetValue(key, out var accumulator))
                {
                    aggregate[key] = accumulator = new Accumulator(key);
                }

                accumulator.Record(speed, Encoding.UTF8.GetString(licensePlate));

                span = span.Slice(newline + 1);
            }

            read = stream.Read(buffer);
        }
        
        done:
        return new ValueTask<Dictionary<string, Accumulator>>(aggregate);
    }
}
