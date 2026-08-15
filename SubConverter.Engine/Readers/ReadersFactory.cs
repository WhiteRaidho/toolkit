namespace SubConverter.Engine.Readers;

public static class ReadersFactory
{
    public static IReader GetReader(SupportedFormat format)
    {
        return format switch
        {
            SupportedFormat.srt => new SrtReader(),
            SupportedFormat.sub => new MicroDVDReader(),
            _ => throw new NotImplementedException($"Reader for [{format}] not implemented yet")
        };
    }
}