
using System.ComponentModel;

namespace SubConverter.Engine.Readers;

public class MicroDVDReader : IReader
{
    public List<InternalSubtitle> Read(FileInfo file, AdditionalInfo info)
    {
        if(!File.Exists(file.FullName))
        {
            throw new FileNotFoundException($"File not found: {file.FullName}");
        }

        var result = new List<InternalSubtitle>();

        var lines = File.ReadAllLines(file.FullName, System.Text.Encoding.UTF8);

        for(int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            int startFrame = ReadFrame(i, line, out int startFrameEnd, 0);
            int endFrame = ReadFrame(i, line, out int endFrameEnd, startFrameEnd);
            endFrameEnd += 1;   // Increase by 1 to account for the '}'

            string text = string.Join("\r\n", line[endFrameEnd..].Split('|'));
            
            TimeSpan start = TimeSpan.FromSeconds(startFrame / info.Framerate);
            TimeSpan end = TimeSpan.FromSeconds(endFrame / info.Framerate);

            result.Add(new(i + 1, start, end, text));
        }

        return result;
    }

    private static int ReadFrame(int lineNumber, string line, out int frameEnd, int searchStartIndex = 0)
    {
        int startIndex = line.IndexOf('{', searchStartIndex);
        if(startIndex < 0) throw new FormatException($"Wrong format in line {lineNumber}, expected frame in format {{000}}");
        int endIndex = line.IndexOf('}', startIndex);
        if(endIndex < 0) throw new FormatException($"Wrong format in line {lineNumber}, expected frame in format {{000}}");
        frameEnd = endIndex;
        startIndex += 1;    // Increase by 1 to account for the '{'
        if(!int.TryParse(line[startIndex..endIndex], out int frame)) throw new FormatException($"Wrong format in line {lineNumber}, expected frame in fromat {{000}}");
        return frame;
    }
}