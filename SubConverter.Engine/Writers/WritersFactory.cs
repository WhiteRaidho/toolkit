namespace SubConverter.Engine.Writers;

public static class WritersFactory {
    public static IWriter GetWriter(SupportedFormat format)
    {
        return format switch
        {
            SupportedFormat.srt => new SrtWriter(),
            SupportedFormat.sub => new MicroDVDWriter(),
            SupportedFormat.vtt => new WebVTTWriter(),
            _ => throw new NotImplementedException($"Writer for [{format}] not implemented yet")
        };
    }
}