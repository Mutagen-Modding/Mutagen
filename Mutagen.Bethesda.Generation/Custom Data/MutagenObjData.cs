using Loqui.Generation;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NET5_0 
using TaskCompletionSource = System.Threading.Tasks.TaskCompletionSource; 
#else 
using TaskCompletionSource = Noggog.TaskCompletionSource;
#endif

namespace Mutagen.Bethesda.Generation
{
    public class MutagenObjData
    {
        public ObjectGeneration ObjGen { get; private set; }
        public RecordType? RecordType;
        public RecordType? OverflowRecordType;
        public bool FailOnUnknown;
        public ObjectType? ObjectType;
        public RecordType? MarkerType;
        public HashSet<RecordType> TriggeringRecordTypes = new HashSet<RecordType>();
        public HashSet<RecordType> CustomRecordTypeTriggers = new HashSet<RecordType>();
        public string TriggeringSource;
        public bool CustomBinary;
        public BinaryGenerationType BinaryOverlay = BinaryGenerationType.Normal;
        public CustomEnd CustomBinaryEnd;
        public Task<IEnumerable<KeyValuePair<IEnumerable<RecordType>, ObjectGeneration>>> GenerationTypes => GetGenerationTypes();
        public TaskCompletionSource WiringComplete = new TaskCompletionSource();
        public TaskCompletionSource DataTypeModuleComplete = new TaskCompletionSource();
        public RecordTypeConverter BaseRecordTypeConverter;
        public Dictionary<GameRelease, RecordTypeConverter> GameReleaseConverters;
        public Dictionary<byte, RecordTypeConverter> VersionConverters;
        public HashSet<GameRelease> GameReleaseOptions;
        public RecordType? EndMarkerType;
        public bool MajorRecordFlags;
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
        // Hacky coincidental trick
        public bool HasMultipleReleases => GameCategory?.HasFormVersion() ?? false;

        public MutagenObjData(ObjectGeneration objGen)
        {
            this.ObjGen = objGen;
        }

        private async Task<IEnumerable<KeyValuePair<IEnumerable<RecordType>, ObjectGeneration>>> GetGenerationTypes()
        {
            List<KeyValuePair<IEnumerable<RecordType>, ObjectGeneration>> ret = new List<KeyValuePair<IEnumerable<Bethesda.RecordType>, ObjectGeneration>>()
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
}
