
using System.Text;

namespace SubConverter.Writers;

public class SrtWriter : IWriter
{
    public void Write(FileInfo file, List<InternalSubtitle>? subtitles, AdditionalInfo info)
    {
        if(subtitles is null || subtitles.Count == 0) return;

        StringBuilder sb = new();

        foreach(var sub in subtitles)
        {
            sb.AppendLine(sub.Index.ToString());
            sb.AppendLine($@"{sub.Start:hh\:mm\:ss\,fff} --> {sub.End:hh\:mm\:ss\,fff}");
            sb.AppendLine(sub.Text);
            sb.AppendLine();
        }

        File.WriteAllText(file.FullName, sb.ToString());
    }
}