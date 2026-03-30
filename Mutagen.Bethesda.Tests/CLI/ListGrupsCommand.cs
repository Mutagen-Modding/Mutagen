#nullable enable
using CommandLine;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;

namespace Mutagen.Bethesda.Tests.CLI;

[Verb("list-grups", HelpText = "List all top-level GRUPs in a file")]
public class ListGrupsCommand
{
    [Option('p', "path", Required = true, HelpText = "Path to the binary file")]
    public string Path { get; set; } = string.Empty;

    [Option('r', "release", Required = true, HelpText = "GameRelease enum value")]
    public GameRelease Release { get; set; }

    public int Run()
    {
        try
        {
            var meta = GameConstants.Get(Release);
            ReadOnlyMemorySlice<byte> data = File.ReadAllBytes(Path);

            // Skip the TES4/mod header
            var modHeader = meta.MajorRecordHeader(data);
            long pos = modHeader.HeaderLength + modHeader.ContentLength;

            Console.WriteLine($"  {"Type",-6} {"Position",-12} {"Size",-14} {"Records"}");
            Console.WriteLine($"  {"----",-6} {"--------",-12} {"----",-14} {"-------"}");

            while (pos < data.Length)
            {
                var header = meta.GroupHeader(data.Slice((int)pos));
                if (!header.IsGroup) break;

                // Count major records in this group
                var frame = new GroupFrame(header, data.Slice((int)pos));
                var recordCount = frame.EnumerateMajorRecords().Count();

                Console.WriteLine($"  {header.ContainedRecordType,-6} 0x{pos:X8}   0x{header.TotalLength:X8} ({header.TotalLength,9})  {recordCount}");
                pos += header.TotalLength;
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
