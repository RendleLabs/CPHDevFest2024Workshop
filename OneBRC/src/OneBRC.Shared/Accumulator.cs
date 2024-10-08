namespace OneBRC.Shared;

public class Accumulator
{
    public string City { get; }
    private float _total;
    private int _count;
    private float _min = float.MaxValue;
    private float _max = float.MinValue;

    public Accumulator(string city)
    {
        City = city;
    }

    public void Record(float value)
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

    public float Mean => _total / _count;
    public float Min => _min;
    public float Max => _max;
    public int Count => _count;
}