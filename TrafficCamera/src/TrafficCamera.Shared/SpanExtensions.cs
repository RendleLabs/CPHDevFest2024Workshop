namespace TrafficCamera.Shared;

public static class SpanExtensions
{
    public static void Split(this Span<byte> span, Span<Range> ranges, byte separator)
    {
        Split((ReadOnlySpan<byte>)span, ranges, separator);
    }
    public static void Split(this ReadOnlySpan<byte> span, Span<Range> ranges, byte separator)
    {
        int index = 0;
        int start = 0;
        while (true)
        {
            int sep = span.Slice(start).IndexOf(separator);
            if (sep == -1)
            {
                ranges[index] = new Range(start, span.Length);
                break;
            }

            ranges[index] = new Range(start, start + sep);
            start += sep + 1;
            index++;
        }
    }
}