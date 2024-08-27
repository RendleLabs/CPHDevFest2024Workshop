using System.Diagnostics;
using System.Runtime.Intrinsics;
using TrafficCamera;
using TrafficCamera.Shared;

var stopwatch = Stopwatch.StartNew();

var filePath = Path.GetFullPath(args[0]);

var impl = new ThreadedImpl(filePath);
var t = impl.Run();

var total = Vector256.Create<int>(0);

foreach (var (road, acc) in t.Result.OrderBy(p => p.Key))
{
    var colors = acc.GetColors();
    // total = Vector256.Add(total, colors);
    total += colors;
}

for (int i = 0; i < 8; i++)
{
    Console.WriteLine($"{Colors.Name(i)}: {total[i]}");
}

stopwatch.Stop();

Console.WriteLine();
Console.WriteLine($"Elapsed time: {stopwatch.Elapsed}");