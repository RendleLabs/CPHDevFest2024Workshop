using System.IO.MemoryMappedFiles;

namespace OneBRC;

public static class MemoryMappedFileAnalyzer
{
    public static unsafe MemoryMappedFileChunk[] GetChunks(this MemoryMappedFile mmf, long size, int threadCount)
    {
        var chunks = new MemoryMappedFileChunk[threadCount];
        
        using var accessor = mmf.CreateViewAccessor(0, size, MemoryMappedFileAccess.Read);
        byte* pointer = null;
        accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref pointer);

        var estimatedChunkSize = (int)(size / threadCount);
        
        long offset = 0;

        for (int i = 0; i < threadCount - 1; i++)
        {
            var span = new ReadOnlySpan<byte>(pointer + offset, estimatedChunkSize);
            var lastNewline = span.LastIndexOf((byte)'\n');
            var actualSize = lastNewline + 1;
            chunks[i] = new MemoryMappedFileChunk(offset, actualSize, i);
            var x = new Span<byte>(pointer + chunks[i].Offset, chunks[i].Length)[^1];
            if (x != 10) throw new Exception("Fuck");
            offset += actualSize;
        }
        
        var last = chunks[^1] = new MemoryMappedFileChunk(offset, size - offset, chunks.Length - 1);

        // var y = new ReadOnlySpan<byte>(pointer + last.Offset, (int)last.Length);
        // var yy = y[^1];

        return chunks;
    }
}