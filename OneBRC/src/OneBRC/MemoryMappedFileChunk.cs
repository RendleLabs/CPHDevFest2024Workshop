namespace OneBRC;

public readonly struct MemoryMappedFileChunk
{
    public readonly long Offset;
    public readonly int Length;
    public readonly int Index;

    public MemoryMappedFileChunk(long offset, long length, int index)
    {
        Offset = offset;
        Length = (int)length;
        Index = index;
    }
}