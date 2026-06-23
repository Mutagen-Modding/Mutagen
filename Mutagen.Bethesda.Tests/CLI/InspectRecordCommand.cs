#nullable enable
using CommandLine;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;

namespace Mutagen.Bethesda.Tests.CLI;

[Verb("inspect-record", HelpText = "Read major record + list subrecords at a binary position")]
public class InspectRecordCommand
{
    [Option('p', "path", Required = true, HelpText = "Path to the binary file")]
    public string Path { get; set; } = string.Empty;

    [Option('r', "release", Required = true, HelpText = "GameRelease enum value")]
    public GameRelease Release { get; set; }

    [Option("pos", Required = true, HelpText = "Position in file (hex 0x... or decimal)")]
    public string Position { get; set; } = string.Empty;

    [Option('n', "preview-bytes", Default = 16, HelpText = "Number of preview bytes per subrecord")]
    public int PreviewBytes { get; set; }

    public int Run()
    {
        try
        {
            var meta = GameConstants.Get(Release);
            var pos = CliHelpers.ParsePosition(Position);
            ReadOnlyMemorySlice<byte> data = File.ReadAllBytes(Path);
            var frame = meta.MajorRecord(data.Slice((int)pos));

            Console.WriteLine($"Record Type:   {frame.RecordType}");
            Console.WriteLine($"FormID:        {frame.FormID}");
            Console.WriteLine($"Flags:         0x{frame.Header.MajorRecordFlags:X}");
            Console.WriteLine($"Content Length:0x{frame.ContentLength:X} ({frame.ContentLength})");
            Console.WriteLine($"Total Length:  0x{frame.TotalLength:X} ({frame.TotalLength})");
            Console.WriteLine();
            Console.WriteLine("Subrecords:");
            Console.WriteLine($"  {"Type",-6} {"Offset",-10} {"Size",-8} Preview");
            Console.WriteLine($"  {"----",-6} {"------",-10} {"----",-8} -------");

            foreach (var sub in frame)
            {
                var preview = sub.Content.Slice(0, Math.Min(PreviewBytes, sub.ContentLength));
                var hex = string.Join(" ", preview.ToArray().Select(b => b.ToString("X2")));
                Console.WriteLine($"  {sub.RecordType,-6} 0x{sub.Location:X6}   {sub.ContentLength,-8} {hex}");
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return -1;
        }
    }
}
