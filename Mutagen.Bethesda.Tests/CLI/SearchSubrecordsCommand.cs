#nullable enable
using CommandLine;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;

namespace Mutagen.Bethesda.Tests.CLI;

[Verb("search-subrecords", HelpText = "Search for subrecord patterns within a GRUP")]
public class SearchSubrecordsCommand
{
    [Option('p', "path", Required = true, HelpText = "Path to the binary file")]
    public string Path { get; set; } = string.Empty;

    [Option('r', "release", Required = true, HelpText = "GameRelease enum value")]
    public GameRelease Release { get; set; }

    [Option("grup-pos", Required = true, HelpText = "Position of the GRUP header (hex 0x... or decimal)")]
    public string GrupPosition { get; set; } = string.Empty;

    [Option('s', "subrecord", Required = true, HelpText = "4-char subrecord type to search for (e.g. RNAM)")]
    public string SubrecordType { get; set; } = string.Empty;

    [Option("all-null", Default = false, HelpText = "Only show subrecords where ALL content bytes are 0x00")]
    public bool AllNull { get; set; }

    [Option("summary", Default = false, HelpText = "Show summary of content lengths instead of individual results")]
    public bool Summary { get; set; }

    public int Run()
    {
        try
        {
            var meta = GameConstants.Get(Release);
            var grupPos = (int)CliHelpers.ParsePosition(GrupPosition);
            ReadOnlyMemorySlice<byte> data = File.ReadAllBytes(Path);
            var grup = meta.Group(data.Slice(grupPos));

            var targetType = new Plugins.RecordType(SubrecordType);
            var lengthCounts = new Dictionary<int, int>();
            var allNullByLength = new Dictionary<int, int>();
            int totalFound = 0;
            int totalAllNull = 0;

            foreach (var majorPin in grup.EnumerateMajorRecords())
            {
                var filePos = grupPos + majorPin.Location;
                foreach (var sub in majorPin.Frame)
                {
                    if (sub.RecordType != targetType) continue;

                    totalFound++;
                    var len = sub.ContentLength;

                    if (!lengthCounts.ContainsKey(len))
                        lengthCounts[len] = 0;
                    lengthCounts[len]++;

                    bool isAllNull = sub.Content.ToArray().All(b => b == 0);
                    if (isAllNull)
                    {
                        totalAllNull++;
                        if (!allNullByLength.ContainsKey(len))
                            allNullByLength[len] = 0;
                        allNullByLength[len]++;
                    }

                    if (!Summary && (!AllNull || isAllNull))
                    {
                        var preview = sub.Content.Slice(0, Math.Min(32, sub.ContentLength));
                        var hex = string.Join(" ", preview.ToArray().Select(b => b.ToString("X2")));
                        Console.WriteLine(
                            $"  0x{filePos + sub.Location:X6}  len={len,-4} {(isAllNull ? "[ALL NULL]" : "")} {hex}");
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine($"=== {SubrecordType} Summary ===");
            Console.WriteLine($"Total found: {totalFound}");
            Console.WriteLine($"Total all-null: {totalAllNull}");
            Console.WriteLine();
            Console.WriteLine("Content length distribution:");
            foreach (var kv in lengthCounts.OrderBy(x => x.Key))
            {
                var nullCount = allNullByLength.GetValueOrDefault(kv.Key, 0);
                Console.WriteLine($"  len={kv.Key}: {kv.Value} total, {nullCount} all-null");
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
