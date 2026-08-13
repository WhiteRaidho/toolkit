namespace SubConverter.Writers;

public static class WritersFactory {
    public static IWriter GetWriter(SupportedFormat format)
    {
        return format switch
        {
            SupportedFormat.srt => new SrtWriter(),
            _ => throw new NotImplementedException($"Writer for [{format}] not implemented yet")
        };
    }
}