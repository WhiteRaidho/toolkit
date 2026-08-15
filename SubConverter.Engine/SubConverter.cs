using SubConverter.Engine.Readers;
using SubConverter.Engine.Writers;

namespace SubConverter.Engine;

public static class SubtitlesConverter
{
    public static void Convert(FileInfo inputFile, SupportedFormat inputFormat, FileInfo outputFile, SupportedFormat outputFormat, AdditionalInfo info)
    {
        IReader reader = ReadersFactory.GetReader(inputFormat);
        List<InternalSubtitle> subtitles = reader.Read(inputFile, info);

        if (info.Offset != TimeSpan.Zero && subtitles is not null)
        {
            var newSubtitles = new List<InternalSubtitle>();
            foreach (var sub in subtitles)
            {
                var start = sub.Start + info.Offset;
                var end = sub.End + info.Offset;
                newSubtitles.Add(new(sub.Index, start, end, sub.Text));
            }
            subtitles = newSubtitles;
        }

        IWriter writer = WritersFactory.GetWriter(outputFormat);
        writer.Write(outputFile, subtitles, info);
    }
}
