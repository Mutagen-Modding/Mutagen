#nullable enable
using CommandLine;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;

namespace Mutagen.Bethesda.Tests.CLI;

[Verb("inspect-grup", HelpText = "Read GRUP header at a binary position")]
public class InspectGrupCommand
{
    [Option('p', "path", Required = true, HelpText = "Path to the binary file")]
    public string Path { get; set; } = string.Empty;

    [Option('r', "release", Required = true, HelpText = "GameRelease enum value")]
    public GameRelease Release { get; set; }

    [Option("pos", Required = true, HelpText = "Position in file (hex 0x... or decimal)")]
    public string Position { get; set; } = string.Empty;

    public int Run()
    {
        try
        {
            var meta = GameConstants.Get(Release);
            var pos = CliHelpers.ParsePosition(Position);
            ReadOnlyMemorySlice<byte> data = File.ReadAllBytes(Path);
            var header = meta.GroupHeader(data.Slice((int)pos));

            Console.WriteLine($"Record Type:   {header.RecordType}");
            Console.WriteLine($"Contained Type:{header.ContainedRecordType}");
            Console.WriteLine($"Group Type:    {header.GroupType}");
            Console.WriteLine($"Total Length:  0x{header.TotalLength:X} ({header.TotalLength})");
            Console.WriteLine($"Content Length:0x{header.ContentLength:X} ({header.ContentLength})");
            Console.WriteLine($"Content Start: 0x{pos + header.HeaderLength:X}");
            Console.WriteLine($"Content End:   0x{pos + header.TotalLength:X}");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return -1;
        }
    }
}
