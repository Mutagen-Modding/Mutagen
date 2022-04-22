using Loqui.Generation;
using Mutagen.Bethesda.Generation.Modules.Binary;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class MutagenFieldData
{
    public readonly TypeGeneration SourceTypeGeneration;
    public TypeGeneration Parent;
    public RecordType? MarkerType { get; set; }
    public RecordType? RecordType { get; set; }
    public RecordType? OverflowRecordType { get; set; }
    public HashSet<RecordType> TriggeringRecordTypes { get; } = new();
    public HashSet<string> TriggeringRecordAccessors = new();
    public string TriggeringRecordSetAccessor { get; set; }
    public string? TriggeringRecordAccessor { get; set; }
    public bool HasTrigger => this.TriggeringRecordAccessors.Count > 0 || SubLoquiTypes.Count > 0;
    public bool HandleTrigger = true;
    public int? Length;
    public BinaryGenerationType Binary;
    public BinaryGenerationType? BinaryOverlay;
    public BinaryGenerationType BinaryOverlayFallback => this.BinaryOverlay ?? this.Binary;
    public bool CustomFolder;
    public Dictionary<RecordType, List<ObjectGeneration>> SubLoquiTypes = new();
    public IEnumerable<KeyValuePair<IEnumerable<RecordType>, TypeGeneration>> GenerationTypes => GetGenerationTypes();
    public bool IsTriggerForObject;
    public RecordTypeConverter RecordTypeConverter;
    public ushort? CustomVersion;
    public List<(ushort Version, VersionAction Action)> Versioning = new();
    public bool HasVersioning => Versioning.Count > 0;
    public bool IsAfterBreak;

    public MutagenFieldData(TypeGeneration source)
    {
        this.SourceTypeGeneration = source;
    }

    private IEnumerable<KeyValuePair<IEnumerable<RecordType>, TypeGeneration>> GetGenerationTypes()
    {
        if (this.TriggeringRecordTypes.Count > 0)
        {
            yield return new KeyValuePair<IEnumerable<RecordType>, TypeGeneration>(
                this.TriggeringRecordTypes,
                this.SourceTypeGeneration);
        }
        if (!(this.SourceTypeGeneration is LoquiType loqui)) yield break;
        foreach (var subType in this.SubLoquiTypes
                     .SelectMany(x => x.Value.Select(l => new KeyValuePair<RecordType, ObjectGeneration>(x.Key, l)))
                     .GroupBy((g) => g.Value))
        {
            yield return new KeyValuePair<IEnumerable<RecordType>, TypeGeneration>(
                subType.Select((s) => s.Key),
                loqui.Spawn(subType.Key));
        }
    }
}