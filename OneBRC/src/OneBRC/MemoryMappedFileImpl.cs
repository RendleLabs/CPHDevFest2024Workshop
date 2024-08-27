// using System.IO.MemoryMappedFiles;
// using System.Text;
// using OneBRC.Shared;
//
// namespace OneBRC;
//
// public class MemoryMappedFileImpl
// {
//     private readonly string _filepath;
//
//     public MemoryMappedFileImpl(string filepath)
//     {
//         _filepath = filepath;
//     }
//
//     public unsafe ValueTask<Dictionary<string, Accumulator>> Run()
//     {
//         var size = new FileInfo(_filepath).Length;
//         var mmf = MemoryMappedFile.CreateFromFile(_filepath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
//
//         var view = mmf.CreateViewAccessor(0, size, MemoryMappedFileAccess.Read);
//
//         byte* pointer = null;
//         view.SafeMemoryMappedViewHandle.AcquirePointer(ref pointer);
//         var span = new ReadOnlySpan<byte>(pointer, (int)size);
//         return new ValueTask<Dictionary<string, Accumulator>>(Run(span));
//     }
//
//     private static Dictionary<string, Accumulator> Run(ReadOnlySpan<byte> span)
//     {
//         var aggregate = new Dictionary<CityKey, Accumulator>();
//
//         while (span.Length > 0)
//         {
//             var endOfLine = span.IndexOf((byte)'\n');
//
//             if (endOfLine < 0)
//             {
//                 break;
//             }
//
//             var line = span.Slice(0, endOfLine);
//             ProcessLine(line, aggregate);
//             span = span.Slice(endOfLine + 1);
//         }
//         
//         return aggregate.Values.ToDictionary(a => a.City);
//     }
//     
//     private static void ProcessLine(ReadOnlySpan<byte> line, Dictionary<CityKey, Accumulator> aggregate)
//     {
//         var semicolon = line.IndexOf((byte)';');
//         var name = line.Slice(0, semicolon);
//         var temp = line.Slice(semicolon + 1);
//
//         var cityKey = CityKey.FromSpan(name);
//         var value = float.Parse(temp);
//
//         if (!aggregate.TryGetValue(cityKey, out var accumulator))
//         {
//             accumulator = aggregate[cityKey] = new Accumulator(Encoding.UTF8.GetString(name));
//         }
//         
//         accumulator.Record(value);
//     }
// }