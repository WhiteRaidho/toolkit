
using System.Text;

namespace SubConverter.Engine.Writers;

public class WebVTTWriter : IWriter
{
    public void Write(FileInfo file, List<InternalSubtitle>? subtitles, AdditionalInfo info)
    {
        if(subtitles is null || subtitles.Count == 0) return;

        StringBuilder sb = new();

        sb.AppendLine("WEBVTT" + Environment.NewLine);

        foreach(var sub in subtitles)
        {
            sb.AppendLine($@"{sub.Start:hh\:mm\:ss\,fff} --> {sub.End:hh\:mm\:ss\,fff}");
            sb.AppendLine(sub.Text);
            sb.AppendLine();
        }

        File.WriteAllText(file.FullName, sb.ToString());
    }
}