using System.Diagnostics;
using TrafficCamera;

var stopwatch = Stopwatch.StartNew();

var filePath = Path.GetFullPath(args[0]);

var impl = new StreamImpl(filePath);
// var impl = new MemoryMappedFileImpl(filePath);
var t = impl.Run();

if (!t.IsCompleted)
{
    await t;
}

foreach (var (road, acc) in t.Result.OrderBy(p => p.Key))
{
    Console.WriteLine($"{road}: {acc.Slowest} [{acc.SlowestLicensePlate}] / {acc.Mean} / {acc.Fastest} [{acc.FastestLicensePlate}]");
}

stopwatch.Stop();

Console.WriteLine();
Console.WriteLine($"Elapsed time: {stopwatch.Elapsed}");
