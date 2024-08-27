namespace OneBRC.Shared;

public static class IntParser
{
    public static int Parse(ReadOnlySpan<byte> input)
    {
        int negative = 1;
        if (input[0] == (byte)'-')
        {
            negative = -1;
            input = input.Slice(1);
        }

        int value = 0;

        for (; input.Length > 0; input = input.Slice(1))
        {
            if (input[0] == (byte)'.') continue;
            int n = input[0] - 48;
            value = value * 10 + n;
        }
        
        return value * negative;
    }
}