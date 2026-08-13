namespace SubConverter.Writers;

public interface IWriter
{
    void Write(FileInfo file, List<InternalSubtitle>? subtitles, AdditionalInfo info);
}