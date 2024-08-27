using System.Diagnostics;
using TrafficCamera;

var stopwatch = Stopwatch.StartNew();

var filePath = Path.GetFullPath(args[0]);

var impl = new ThreadedImpl(filePath);
var t = impl.Run();

if (!t.IsCompleted)
{
    await t;
}

foreach (var (road, acc) in t.Result.OrderBy(p => p.Key))
{
    var mean = ((float)acc.Total / acc.Count) / 10f;
    Console.WriteLine(
        $"{road}: {acc.Slowest / 10f:F1} [{acc.SlowestLicensePlate}] / {mean:F1} / {acc.Fastest / 10f:F1} [{acc.FastestLicensePlate}]");
}

stopwatch.Stop();

Console.WriteLine();
Console.WriteLine($"Elapsed time: {stopwatch.Elapsed}");