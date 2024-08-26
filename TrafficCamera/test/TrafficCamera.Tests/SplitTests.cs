using System.Text;
using TrafficCamera.Shared;

namespace TrafficCamera.Tests;

public class SplitTests
{
    [Fact]
    public void SplitsLine()
    {
        ReadOnlySpan<byte> line = "2024-08-26T05:00:07.503;A236;YJ36 TLE;Blue;46.2"u8;

        Span<Range> ranges = stackalloc Range[5];

        line.Split(ranges, (byte)';');

        Assert.Equal("2024-08-26T05:00:07.503", Encoding.UTF8.GetString(line[ranges[0]]));
        Assert.Equal("A236", Encoding.UTF8.GetString(line[ranges[1]]));
        Assert.Equal("YJ36 TLE", Encoding.UTF8.GetString(line[ranges[2]]));
        Assert.Equal("Blue", Encoding.UTF8.GetString(line[ranges[3]]));
        Assert.Equal("46.2", Encoding.UTF8.GetString(line[ranges[4]]));
    }
}