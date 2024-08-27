using System.Diagnostics;
using OneBRC;

var stopwatch = Stopwatch.StartNew();

var filePath = Path.GetFullPath(args[0]);
// var impl = new StreamImpl(filePath);
// var impl = new MemoryMappedFileImpl(filePath);
var impl = new ThreadedImpl(filePath);

var task = impl.Run();

if (!task.IsCompleted)
{
    await task;
}

foreach (var accumulator in task.Result.Values.OrderBy(a => a.City))
{
    float min = accumulator.Min / 1000f;
    float max = accumulator.Max / 1000f;
    float mean = ((float)accumulator.Total / accumulator.Count) / 1000f;
    Console.WriteLine($"{accumulator.City}: {min:F1}/{mean:F1}/{max:F1}");
}

stopwatch.Stop();

Console.WriteLine();
Console.WriteLine($"Done in {stopwatch.Elapsed:g}");
Console.WriteLine();
