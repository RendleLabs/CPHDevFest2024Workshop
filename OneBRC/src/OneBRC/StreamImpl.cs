// using System.Text;
// using OneBRC.Shared;
//
// namespace OneBRC;
//
// public class StreamImpl
// {
//     private readonly string _filepath;
//
//     public StreamImpl(string filepath)
//     {
//         _filepath = filepath;
//     }
//
//     public ValueTask<Dictionary<string, Accumulator>> Run()
//     {
//         var aggregate = new Dictionary<CityKey, Accumulator>();
//         
//         using var stream = File.Open(_filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
//
//         var buffer = new byte[1024];
//
//         int read = stream.Read(buffer, 0, buffer.Length);
//         int lines = 0;
//
//         while (read > 0)
//         {
//             start:
//             
//             var span = buffer.AsSpan(0, read);
//
//             while (span.Length > 0)
//             {
//                 var endOfLine = span.IndexOf((byte)'\n');
//
//                 if (endOfLine < 0)
//                 {
//                     span.CopyTo(buffer);
//                     var readSpan = buffer.AsSpan(span.Length);
//                     read = stream.Read(readSpan);
//                     if (read == 0)
//                     {
//                         goto done;
//                     }
//
//                     read += span.Length;
//                     goto start;
//                 }
//
//                 ++lines;
//                 
//                 var line = span.Slice(0, endOfLine);
//                 ProcessLine(line, aggregate);
//                 span = span.Slice(endOfLine + 1);
//             }
//
//             read = stream.Read(buffer);
//         }
//         
//         done:
//         return new ValueTask<Dictionary<string, Accumulator>>(
//             aggregate.Values.ToDictionary(a => a.City));
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