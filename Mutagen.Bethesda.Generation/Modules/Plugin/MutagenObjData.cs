using Loqui.Generation;
using Mutagen.Bethesda.Generation.Modules.Binary;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Internals;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class MutagenObjData
{
    public ObjectGeneration ObjGen { get; }
    public RecordType? RecordType { get; set; }
    public RecordType? OverflowRecordType;
    public bool FailOnUnknown;
    public ObjectType? ObjectType;
    public RecordType? MarkerType;
    public RecordCollection TriggeringRecordTypes = new();
    public RecordCollection CustomRecordTypeTriggers = new();
    public RecordCollection AdditionalContainedRecordTypes = new();
    public string TriggeringSource;
    public bool CustomBinary;
    public BinaryGenerationType BinaryOverlay = BinaryGenerationType.Normal;
    public bool BinaryOverlayGenerateCtor;
    public CustomEnd CustomBinaryEnd;
    public Task<IEnumerable<KeyValuePair<IEnumerable<RecordType>, ObjectGeneration>>> GenerationTypes => GetGenerationTypes();
    public TaskCompletionSource WiringComplete = new();
    public TaskCompletionSource DataTypeModuleComplete = new();
    public RecordTypeConverter BaseRecordTypeConverter;
    public Dictionary<GameRelease, RecordTypeConverter>? GameReleaseConverters;
    public Dictionary<byte, RecordTypeConverter>? VersionConverters;
    public HashSet<GameRelease> GameReleaseOptions;
    public RecordType? EndMarkerType;
    public bool MajorRecordFlags;
    public bool PartialForm;
    public int? LastRequiredFieldIndex;
    public bool ShortCircuitToLastRequiredField;
    public int? GetLastRequiredFieldIndexToUse() => ShortCircuitToLastRequiredField ? LastRequiredFieldIndex : null;
    public GameCategory? GameCategory
    {
        get
        {
            if (Enum.TryParse<Bethesda.GameCategory>(ObjGen.Namespace.Split('.').Last(), out var mode))
            {
                return mode;
            }
            return null;
        }
    }
    public bool CustomRecordFallback;
    public bool UsesStringFiles = true;
    public bool HasMultipleReleases
    {
        get
        {
            var name = $"{ObjGen.ProtoGen.Protocol.Namespace}Mod";
            if (!ObjGen.ProtoGen.ObjectGenerationsByName.TryGetValue(name, out var modObj)) return false;
            return ObjGen.ProtoGen.ObjectGenerationsByName[name].GetObjectData().GameReleaseOptions != null;
        }
    }

    public List<int> Subgroups = new();

    public bool HasInferredAssets;

    public bool HasResolvedAssets;
    public bool AbstractSplitter;

    public MutagenObjData(ObjectGeneration objGen)
    {
        this.ObjGen = objGen;
    }

    private async Task<IEnumerable<KeyValuePair<IEnumerable<RecordType>, ObjectGeneration>>> GetGenerationTypes()
    {
        List<KeyValuePair<IEnumerable<RecordType>, ObjectGeneration>> ret = new()
        {
            new KeyValuePair<IEnumerable<RecordType>, ObjectGeneration>(
                this.TriggeringRecordTypes,
                this.ObjGen)
        };
        foreach (var subObjs in await this.ObjGen.InheritingObjects())
        {
            ret.Add(new KeyValuePair<IEnumerable<RecordType>, ObjectGeneration>(
                await subObjs.GetTriggeringRecordTypes(),
                subObjs));
        }
        return ret;
    }

    public bool HasVersioning() => this.ObjGen.AllFields.Any(f => f is BreakType);
}