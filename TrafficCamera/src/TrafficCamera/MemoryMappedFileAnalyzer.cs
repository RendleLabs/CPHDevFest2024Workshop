using System.IO.MemoryMappedFiles;

namespace TrafficCamera;

public static class MemoryMappedFileAnalyzer
{
    public static unsafe MemoryMappedFileChunk[] GetChunks(this MemoryMappedFile mmf, long size, int threadCount, byte delimiter)
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
            var lastDelimiter = span.LastIndexOf(delimiter);
            var actualSize = lastDelimiter + 1;
            chunks[i] = new MemoryMappedFileChunk(offset, actualSize, i);
            offset += actualSize;
        }
        
        var last = chunks[^1] = new MemoryMappedFileChunk(offset, size - offset, chunks.Length - 1);

        // var y = new ReadOnlySpan<byte>(pointer + last.Offset, (int)last.Length);
        // var yy = y[^1];

        return chunks;
    }
}