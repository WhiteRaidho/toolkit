using System.CommandLine;
using SubConverter;
using SubConverter.Readers;
using SubConverter.Writers;


RootCommand rootCommand = new("Subtitles command-line converter");

Option<FileInfo> inputFileOption = new("--input", "-i")
{
    Description = "Path to input file",
    Required = true,
};
inputFileOption.AcceptExistingOnly();

Option<FileInfo> outputFileOption = new("--output", "-o")
{
    Description = "Path to output file",
    Required = true
};
outputFileOption.AcceptLegalFileNamesOnly();
outputFileOption.AcceptLegalFilePathsOnly();

Option<SupportedFormat?> inputFormatOption = new("--input-format", "-if")
{
    Description = "Format of input file, if not specified defaults to file extension",
    DefaultValueFactory = _ => null
};

Option<SupportedFormat> outputFormatOption = new("--output-format", "-of")
{
    Description = "Format of output file, if not specified defaults to .srt",
    DefaultValueFactory = _ => SupportedFormat.srt
};

Option<float> framerateOption = new("--framerate", "-fr")
{
    Description = "Framerate used when calculating time for frame specified formats (like MicroDVD .sub). Popular framerates: 23.98, 24, 25, 48, 60",
    DefaultValueFactory = _ => 24
};

Option<TimeSpan> offestOption = new("--offset")
{
    Description = "Specify offset for the subtitles, negative offset makes them more accelerated relative to audio, positive makes them delayed",
    DefaultValueFactory = _ => TimeSpan.Zero
};


rootCommand.Options.Add(inputFileOption);
rootCommand.Options.Add(inputFormatOption);

rootCommand.Options.Add(outputFileOption);
rootCommand.Options.Add(outputFormatOption);

rootCommand.Options.Add(framerateOption);
rootCommand.Options.Add(offestOption);

rootCommand.SetAction(parseResult =>
{
    var inputFile = parseResult.GetRequiredValue(inputFileOption);
    var outputFile = parseResult.GetRequiredValue(outputFileOption);

    var inputFormat = parseResult.GetValue(inputFormatOption);
    var outputFormat = parseResult.GetValue(outputFormatOption);

    AdditionalInfo info = new()
    {
        Framerate = parseResult.GetValue(framerateOption),
        Offset = parseResult.GetValue(offestOption)
    };

    try
    {
        inputFormat = GetInputFormat(inputFormat, inputFile);
    }
    catch (FormatException ex)
    {
        WriteError(ex);
    }

    Console.WriteLine("Reading input file");
    List<InternalSubtitle>? subtitles = null;
    IReader reader = ReadersFactory.GetReader(inputFormat.GetValueOrDefault());
    try
    {
        subtitles = reader.Read(inputFile, info);
        Console.WriteLine($"Found {subtitles.Count} subtitles");
    }
    catch (FormatException ex)
    {
        WriteError(ex);
    }

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

    Console.WriteLine($"Converting to: [{outputFormat}] and saving to file: {outputFile}");
    IWriter writer = WritersFactory.GetWriter(outputFormat);
    writer.Write(outputFile, subtitles, info);
});

return rootCommand.Parse(args).Invoke();


static SupportedFormat GetInputFormat(SupportedFormat? format, FileInfo file)
{
    if (format is not null)
        return format.Value;

    Console.WriteLine("Getting input format based on input file format");
    var extension = Path.GetExtension(file.FullName).TrimStart('.').ToLower();
    if (!Enum.TryParse<SupportedFormat>(extension.ToLower(), out SupportedFormat foundFormat))
        throw new FormatException($"[{extension}] [{file.FullName}] is not supported");
    Console.WriteLine($"Found format: [{foundFormat}]");

    return foundFormat;
}

static void WriteError(Exception ex)
{
    var currentColor = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Error.WriteLine(ex.Message);
    Console.ForegroundColor = currentColor;
}
