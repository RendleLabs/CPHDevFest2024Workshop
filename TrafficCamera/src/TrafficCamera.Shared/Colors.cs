namespace TrafficCamera.Shared;

public class Colors
{
// string[] colors = ["Red", "Green", "Blue", "Black", "White", "Grey", "Silver", "Other"];
    public const int Red = 0;
    public const int Green = 1;
    public const int Blue = 2;
    public const int Black = 3;
    public const int White = 4;
    public const int Grey = 5;
    public const int Silver = 6;
    public const int Other = 7;

    public static string Name(int color)
    {
        return color switch
        {
            0 => "Red",
            1 => "Green",
            2 => "Blue",
            3 => "Black",
            4 => "White",
            5 => "Grey",
            6 => "Silver",
            _ => "Other",
        };
    }

    public static int Parse(ReadOnlySpan<byte> span)
    {
        return span[0] switch
        {
            (byte)'R' => Red,
            (byte)'G' when (span.Length == 5) => Green,
            (byte)'G' when (span.Length == 4) => Grey,
            (byte)'B' when (span.Length == 5) => Black,
            (byte)'B' when (span.Length == 4) => Blue,
            (byte)'W' => White,
            (byte)'S' => Silver,
            _ => Other
        };
    }
}