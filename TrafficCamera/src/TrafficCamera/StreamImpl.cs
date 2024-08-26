﻿using System.Text;
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
        
        Span<Range> ranges = stackalloc Range[5];

        while (read > 0)
        {
            start:
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
