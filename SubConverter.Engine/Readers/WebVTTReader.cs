
using System.Text;

namespace SubConverter.Engine.Readers;

public class WebVTTReader : IReader
{
    public List<InternalSubtitle> Read(FileInfo file, AdditionalInfo info)
    {
        if(!File.Exists(file.FullName))
        {
            throw new FileNotFoundException($"File not found: {file.FullName}");
        }

        var result = new List<InternalSubtitle>();

        var lines = File.ReadAllLines(file.FullName, System.Text.Encoding.UTF8);

        if(lines is null || lines.Length == 0) return [];

        int linePointer = 0;

        InternalSubtitle? block;
        int index = 1;
        do
        {
            block = ReadNextBlock(index++, lines, ref linePointer);
            if(block is not null) result.Add(block);
        } while(block is not null && linePointer < lines.Length);

        return result;
    }

    private static InternalSubtitle? ReadNextBlock(int index, string[] lines, ref int linePointer)
    {
        void CheckEndOfFile(int lineIndex)
        {
            if(lineIndex >= lines.Length)
            {
                throw new EndOfStreamException("Unexpected EoF, cannot read next block");
            }
        }

        // Skip empty lines
        while(string.IsNullOrWhiteSpace(lines[linePointer].Trim()))
        {
            linePointer++;
            if(linePointer >= lines.Length) return null;
        }

        // Read first line as [start] --> [end]
        var timeLine = lines[linePointer++];
        var times = timeLine.Split("-->");
        if(times.Length != 2) throw new FormatException($"Wrong format in line {linePointer - 1}, expected timestamps in format \"[start] --> [end]\", instead got: {timeLine}");
        if(!TimeSpan.TryParse(times[0].Trim(), out TimeSpan start)) throw new FormatException($"Wrong format in line {linePointer - 1}, expected timestamp in format \"00:00:00.000\", instead got: {times[0]}");
        if(!TimeSpan.TryParse(times[1].Trim(), out TimeSpan end)) throw new FormatException($"Wrong format in line {linePointer - 1}, expected timestamp in format \"00:00:00.000\", instead got: {times[1]}");

        CheckEndOfFile(linePointer);

        // Read lines till EoF or empty line reached as text
        StringBuilder sb = new();
        while(linePointer < lines.Length && !string.IsNullOrWhiteSpace(lines[linePointer]))
            sb.Append(lines[linePointer++]);

        return new(index, start, end, sb.ToString().Trim());
    }
}