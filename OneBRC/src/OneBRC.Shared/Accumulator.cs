namespace OneBRC.Shared;

public class Accumulator
{
    public string City { get; }
    private long _total;
    private int _count;
    private int _min = int.MaxValue;
    private int _max = int.MinValue;

    public Accumulator(string city)
    {
        City = city;
    }

    public void Record(int value)
    {
        if (value < _min) _min = value;
        if (value > _max) _max = value;
        _total += value;
        ++_count;
    }
    
    public void Combine(Accumulator other)
    {
        _count += other._count;
        _total += other._total;
        if (other._min < _min) _min = other._min;
        if (other._max < _max) _max = other._max;
    }

    public long Total => _total;
    public int Min => _min;
    public int Max => _max;
    public int Count => _count;
}