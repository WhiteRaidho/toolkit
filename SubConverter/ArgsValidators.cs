using System.Collections.ObjectModel;
using System.CommandLine.Parsing;

namespace SubConverter;

public static class ArgsValidators
{
    public static void FileExistValidator(OptionResult result)
    {
        var file = result.GetValueOrDefault<FileInfo>();
        if(!File.Exists(file.FullName))
            result.AddError($"Input file: {file.FullName} does not exist");
    }
}