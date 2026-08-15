namespace SubConverter.Engine.Readers;

public interface IReader
{
    List<InternalSubtitle> Read(FileInfo file, AdditionalInfo info);
}