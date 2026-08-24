using System.Text;

namespace SubConverter.Engine.Writers;

public class MicroDVDWriter : IWriter
{
    public void Write(FileInfo file, List<InternalSubtitle>? subtitles, AdditionalInfo info)
    {
        if(subtitles is null || subtitles.Count == 0) return;

        StringBuilder sb = new();

        foreach(var sub in subtitles)
        {
            int start = (int)(sub.Start.TotalSeconds * info.Framerate);
            int end   = (int)(sub.End.TotalSeconds   * info.Framerate);
            var text  = string.Join('|', sub.Text.Split("\r\n"));
            sb.AppendLine($"{{{start}}}{{{end}}}{text}");
        }

        File.WriteAllText(file.FullName, sb.ToString());
    }
}