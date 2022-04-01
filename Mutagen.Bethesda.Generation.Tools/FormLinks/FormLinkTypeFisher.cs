using CommandLine;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Analysis;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;

namespace Mutagen.Bethesda.Generation.Tools.FormLinks;

[Verb("formlink-type-fisher")]
public class FormLinkTypeFisher
{
    [Option('f', "File", Required = true, HelpText = "Path to a source file to analyze")]
    public FilePath SourceFile { get; set; }
    
    [Option('r', "Release", Required = true, HelpText = "GameRelease targeted")]
    public GameRelease Release { get; set; }
    
    [Option('m', "Major", Required = true, HelpText = "MajorRecord RecordType to search under")]
    public string MajorRecordType { get; set; }
    
    [Option('s', "Sub", Required = true,  HelpText = "SubRecord RecordType to analyze")]
    public string SubRecordType { get; set; }
    
    [Option('o', "Offset", Required = false, HelpText = "Binary length offset from the start of the subrecord to where the FormLink is")]
    public ushort Offset { get; set; }
    
    public void Execute()
    {
        Console.WriteLine("Finding all record locations...");
        var locs = RecordLocator.GetLocations(
            SourceFile,
            Release);
        var targetedTypes = new HashSet<RecordType>();
        var stream = new MutagenBinaryReadStream(SourceFile, Release);
        Console.WriteLine("Analyzing target links...");
        foreach (var recordLocationMarker in locs.ListedRecords)
        {
            if (recordLocationMarker.Value.Record != MajorRecordType) continue;
            stream.Position = recordLocationMarker.Key;
            var majorFrame = stream.ReadMajorRecordFrame();
            foreach (var subRec in majorFrame.FindEnumerateSubrecords(SubRecordType))
            {
                var link = FormKeyBinaryTranslation.Instance.Parse(
                    subRec.Content.Slice(Offset),
                    stream.MetaData.MasterReferences);
                if (!link.IsNull && locs.TryGetRecord(link, out var otherRec))
                {
                    targetedTypes.Add(otherRec.Record);
                }
            }
        }

        Console.WriteLine($"{MajorRecordType} -> {SubRecordType} {(Offset == 0 ? null : $"at offset {Offset} ")}targeted:");
        foreach (var type in targetedTypes.OrderBy(x => x.Type))
        {
            Console.WriteLine($"  {type}");
        }
        Console.WriteLine("Done");
    }

}