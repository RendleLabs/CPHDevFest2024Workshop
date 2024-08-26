using System.Diagnostics;
using OneBRC;

var stopwatch = Stopwatch.StartNew();

var filePath = Path.GetFullPath(args[0]);
// var impl = new StreamImpl(filePath);
var impl = new MemoryMappedFileImpl(filePath);
var task = impl.Run();

if (!task.IsCompleted)
{
    await task;
}

foreach (var accumulator in task.Result.Values.OrderBy(a => a.City))
{
    Console.WriteLine($"{accumulator.City}: {accumulator.Min:F1}/{accumulator.Mean:F1}/{accumulator.Max:F1}");
}

stopwatch.Stop();

Console.WriteLine();
Console.WriteLine($"Done in {stopwatch.Elapsed:g}");
Console.WriteLine();
