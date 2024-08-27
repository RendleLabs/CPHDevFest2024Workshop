using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Text;

namespace TrafficCamera.Shared;

[InlineArray(8)]
public struct ColorArray
{
    private int _element0;
}

public class IntAccumulator
{
    private ColorArray _colors = new();
    
    public string Road { get; }
    private long _total;
    private int _count;
    private int _slowest = int.MaxValue;
    private string _slowestLicensePlate = string.Empty;
    private int _fastest = int.MinValue;
    private string _fastestLicensePlate = string.Empty;
    
    public IntAccumulator(string road)
    {
        ColorArray colorArray = new ColorArray();
        
        colorArray[0] = 1;
        
        Road = road;
    }

    public void GetColors(Span<int> colors)
    {
        var colorSpan = (ReadOnlySpan<int>)_colors;
        colorSpan.CopyTo(colors);
    }

    public Vector256<int> GetColors()
    {
        var vector = new Vector<int>(_colors);
        return vector.AsVector256();
    }

    public void Record(int value, ReadOnlySpan<byte> licensePlate, int color)
    {
        if (value < _slowest)
        {
            _slowest = value;
            _slowestLicensePlate = Encoding.UTF8.GetString(licensePlate);
        }

        if (value > _fastest)
        {
            _fastest = value;
            _fastestLicensePlate = Encoding.UTF8.GetString(licensePlate);
        }

        _total += value;
        _colors[color] += 1;
        ++_count;
    }
    
    public void Combine(IntAccumulator other)
    {
        _count += other._count;
        _total += other._total;
        if (other._slowest < _slowest)
        {
            _slowest = other._slowest;
            _slowestLicensePlate = other._slowestLicensePlate;
        }

        if (other._fastest < _fastest)
        {
            _fastest = other._fastest;
            _fastestLicensePlate = other._fastestLicensePlate;
        }
        
        for (int i = 0; i < 8; i++)
        {
            _colors[i] += other._colors[i];
        }
    }

    public long Total => _total;
    public long Count => _count;
    public int Slowest => _slowest;
    public string SlowestLicensePlate => _slowestLicensePlate;
    public int Fastest => _fastest;
    public string FastestLicensePlate => _fastestLicensePlate;
}