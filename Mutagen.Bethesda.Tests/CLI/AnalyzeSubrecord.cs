using System.Text;
using CommandLine;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Analysis;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Tests.CLI;

/// <summary>
/// Inspects a subrecord across one or more mods.
/// Dumps the raw bytes (deduplicated, sorted by length) of every occurrence and,
/// for every 4-byte-aligned offset within the subrecord content, attempts to
/// interpret those four bytes as a FormID and reports which record types match
/// against the load order's RecordLocator. High match percentages strongly imply
/// a FormLink at that offset.
/// </summary>
[Verb("analyze-subrecord")]
public class AnalyzeSubrecord
{
    [Option('m', "ModPath", Required = true, HelpText = "Path to the mod whose subrecord content should be inspected")]
    public string PathToMod { get; set; } = string.Empty;

    [Option('r', "Release", Required = true, HelpText = "GameRelease of the mod")]
    public GameRelease Release { get; set; }

    [Option('M', "Major", HelpText = "Optional MajorRecord type to filter by (e.g. PERK)")]
    public string? MajorRecordType { get; set; }

    [Option('s', "Sub", Required = true, HelpText = "Subrecord type to inspect (e.g. PRUC)")]
    public string SubRecordType { get; set; } = string.Empty;

    [Option("max-dump", Default = 32, HelpText = "Maximum number of distinct hex values to dump")]
    public int MaxDump { get; set; }

    [Option("context", HelpText = "Also print the full subrecord-type sequence for each major containing the target subrecord")]
    public bool ShowContext { get; set; }

    public int Execute()
    {
        var release = Release;
        var modPath = new ModPath(PathToMod);

        Console.WriteLine($"Inspecting {SubRecordType} on {(MajorRecordType ?? "<any>")} in {modPath}");

        // Build env over the load order so the FormID lookup table covers all base masters.
        using var env = GameEnvironment.Typical.Builder(release)
            .TransformModListings(x => x.OnlyEnabledAndExisting())
            .Build();

        ILoadOrderGetter<IModFlagsGetter> lo = new LoadOrder<IModFlagsGetter>(
            env.LoadOrder.ListedOrder.ResolveAllModsExist());

        // Build a case-insensitive case-resolving file map for the data folder so we can
        // tolerate Linux filesystems where the masters table casing doesn't match disk.
        var fileMap = Directory.GetFiles(env.DataFolderPath)
            .GroupBy(p => Path.GetFileName(p), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        string ResolveMaster(ModKey key)
        {
            if (fileMap.TryGetValue(key.FileName, out var p)) return p;
            return Path.Combine(env.DataFolderPath, key.FileName);
        }

        // Locate every record in the entire load order so we can identify FormID targets.
        Console.WriteLine("Building FormKey -> RecordType lookup across the load order...");
        var lookup = new Dictionary<FormKey, RecordType>();
        foreach (var listing in env.LoadOrder.ListedOrder.OnlyEnabledAndExisting())
        {
            var listingPath = new ModPath(Path.Combine(env.DataFolderPath, listing.ModKey.FileName));
            if (!File.Exists(listingPath.Path)) continue;
            var locs = RecordLocator.GetLocations(listingPath, release, lo);
            foreach (var marker in locs.ListedRecords.Values)
            {
                lookup.TryAdd(marker.FormKey, marker.Record);
            }
        }
        Console.WriteLine($"  {lookup.Count} records indexed");

        // Read the target mod and collect every matching subrecord's content.
        Console.WriteLine($"Scanning {modPath.ModKey} for {SubRecordType} subrecords...");

        var subType = new RecordType(SubRecordType);
        var majorType = MajorRecordType is null ? (RecordType?)null : new RecordType(MajorRecordType);

        var locations = RecordLocator.GetLocations(modPath, release, lo);
        var contents = new List<byte[]>();
        var contextSequences = new List<(FormKey FormKey, string Sequence)>();

        // Build the master flags lookup ourselves, resolving each master file case-insensitively.
        Cache<IModMasterStyledGetter, ModKey>? masterFlagsLookup = null;
        if (GameConstants.Get(release).SeparateMasterLoadOrders)
        {
            var header = ModHeaderFrame.FromPath(modPath, release);
            masterFlagsLookup = new Cache<IModMasterStyledGetter, ModKey>(x => x.ModKey);
            foreach (var master in header.Masters(modPath.ModKey).Select(x => x.Master))
            {
                var otherHeader = ModHeaderFrame.FromPath(ResolveMaster(master), release);
                masterFlagsLookup.Add(new KeyedMasterStyle(master, otherHeader.MasterStyle));
            }
        }

        using var stream = new MutagenBinaryReadStream(
            modPath,
            ParsingMeta.Factory(BinaryReadParameters.Default with
            {
                MasterFlagsLookup = masterFlagsLookup
            }, release, modPath));

        foreach (var marker in locations.ListedRecords.Values)
        {
            if (majorType != null && marker.Record != majorType.Value) continue;
            stream.Position = marker.Location.Min;
            var majorFrame = stream.ReadMajorRecord();
            if (majorFrame.IsCompressed)
            {
                majorFrame = majorFrame.Decompress(out _);
            }
            bool any = false;
            foreach (var sub in majorFrame.FindEnumerateSubrecords(subType))
            {
                contents.Add(sub.Content.ToArray());
                any = true;
            }

            if (any && ShowContext)
            {
                var seq = string.Join(" ", majorFrame.Select(s => s.RecordType == subType ? $"[{s.RecordType}]" : s.RecordType.Type));
                contextSequences.Add((marker.FormKey, seq));
            }
        }

        Console.WriteLine($"  found {contents.Count} occurrences");

        if (contents.Count == 0)
        {
            return 0;
        }

        if (ShowContext)
        {
            Console.WriteLine();
            Console.WriteLine("Subrecord sequence in containing majors (target marked with []):");
            foreach (var (fk, seq) in contextSequences.Take(10))
            {
                Console.WriteLine($"  {fk}: {seq}");
            }
            if (contextSequences.Count > 10)
            {
                Console.WriteLine($"  ... and {contextSequences.Count - 10} more");
            }
        }

        // Length distribution
        Console.WriteLine();
        Console.WriteLine("Length distribution:");
        foreach (var grp in contents.GroupBy(x => x.Length).OrderBy(x => x.Key))
        {
            Console.WriteLine($"  {grp.Key,4} bytes: {grp.Count()}");
        }

        // Distinct hex dump
        Console.WriteLine();
        Console.WriteLine($"Distinct hex content (first {MaxDump}, sorted by length):");
        var distinct = contents
            .Select(b => Convert.ToHexString(b))
            .Distinct()
            .OrderBy(s => s.Length)
            .ThenBy(s => s)
            .ToList();
        foreach (var hex in distinct.Take(MaxDump))
        {
            Console.WriteLine($"  {hex}  ({hex.Length / 2} bytes)");
        }
        if (distinct.Count > MaxDump)
        {
            Console.WriteLine($"  ... and {distinct.Count - MaxDump} more distinct values");
        }

        // String interpretation
        Console.WriteLine();
        Console.WriteLine("Distinct null-trimmed ASCII interpretation (first 16):");
        foreach (var s in contents
                     .Select(b => TryReadAscii(b))
                     .Where(s => s != null)
                     .Distinct()
                     .Take(16))
        {
            Console.WriteLine($"  \"{s}\"");
        }

        // FormID fishing — try every 4-byte-aligned offset
        Console.WriteLine();
        Console.WriteLine("FormID candidate analysis (per 4-byte aligned offset):");
        var minLen = contents.Min(x => x.Length);
        var masters = stream.MetaData.MasterReferences;
        for (int offset = 0; offset + 4 <= minLen; offset += 4)
        {
            var hits = new Dictionary<RecordType, int>();
            int total = 0;
            int nullCount = 0;
            foreach (var content in contents)
            {
                if (offset + 4 > content.Length) continue;
                total++;
                var fk = FormKeyBinaryTranslation.Instance.Parse(content.AsSpan(offset, 4), masters);
                if (fk.IsNull)
                {
                    nullCount++;
                    continue;
                }
                if (lookup.TryGetValue(fk, out var rec))
                {
                    if (!hits.TryAdd(rec, 1)) hits[rec]++;
                }
            }
            int hitTotal = hits.Values.Sum();
            int pct = total == 0 ? 0 : (int)Math.Round((hitTotal + nullCount) * 100.0 / total);
            var summary = string.Join(", ", hits.OrderByDescending(x => x.Value).Select(x => $"{x.Key}:{x.Value}"));
            if (nullCount > 0) summary = (summary.Length == 0 ? "" : summary + ", ") + $"NULL:{nullCount}";
            Console.WriteLine($"  offset {offset,3}: {pct,3}% likely FormID  ({hitTotal + nullCount}/{total})  [{summary}]");
        }

        return 0;
    }

    private static string? TryReadAscii(byte[] bytes)
    {
        if (bytes.Length == 0) return null;
        int end = bytes.Length;
        if (bytes[^1] == 0) end--;
        for (int i = 0; i < end; i++)
        {
            if (bytes[i] < 0x20 || bytes[i] > 0x7e) return null;
        }
        return end == 0 ? null : Encoding.ASCII.GetString(bytes, 0, end);
    }
}
