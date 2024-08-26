using System.Runtime.InteropServices;

namespace OneBRC.Shared;

public readonly struct CityKey : IEquatable<CityKey>
{
    private readonly long _n1;
    private readonly long _n2;
    private readonly long _n3;
    private readonly long _n4;

    private CityKey(long n1, long n2, long n3, long n4)
    {
        _n1 = n1;
        _n2 = n2;
        _n3 = n3;
        _n4 = n4;
    }

    public static CityKey FromSpan(ReadOnlySpan<byte> span)
    {
        Span<byte> bytes = stackalloc byte[32];
        bytes.Clear();
        span.CopyTo(bytes);

        var n1 = MemoryMarshal.Read<long>(bytes.Slice(0, 8));
        var n2 = MemoryMarshal.Read<long>(bytes.Slice(8, 8));
        var n3 = MemoryMarshal.Read<long>(bytes.Slice(16, 8));
        var n4 = MemoryMarshal.Read<long>(bytes.Slice(24, 8));
        
        return new CityKey(n1, n2, n3, n4);
    }

    public bool Equals(CityKey other)
    {
        return _n1 == other._n1 && _n2 == other._n2 && _n3 == other._n3 && _n4 == other._n4;
    }

    public override bool Equals(object? obj)
    {
        return obj is CityKey other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_n1, _n2, _n3, _n4);
    }

    public static bool operator ==(CityKey left, CityKey right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CityKey left, CityKey right)
    {
        return !left.Equals(right);
    }
}