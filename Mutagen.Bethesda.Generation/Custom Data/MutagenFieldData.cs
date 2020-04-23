using Loqui.Generation;
using Mutagen.Bethesda.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class MutagenFieldData
    {
        public readonly TypeGeneration SourceTypeGeneration;
        public TypeGeneration Parent;
        public RecordType? MarkerType { get; set; }
        public RecordType? RecordType { get; set; }
        public HashSet<RecordType> TriggeringRecordTypes { get; } = new HashSet<Mutagen.Bethesda.RecordType>();
        public HashSet<string> TriggeringRecordAccessors = new HashSet<string>();
        public string TriggeringRecordSetAccessor { get; set; }
        public bool HasTrigger => this.TriggeringRecordAccessors.Count > 0 || SubLoquiTypes.Count > 0;
        public bool HandleTrigger = true;
        public int? Length;
        public BinaryGenerationType Binary;
        public BinaryGenerationType? BinaryOverlay;
        public BinaryGenerationType BinaryOverlayFallback => this.BinaryOverlay ?? this.Binary;
        public bool CustomFolder;
        public Dictionary<RecordType, List<ObjectGeneration>> SubLoquiTypes = new Dictionary<RecordType, List<ObjectGeneration>>();
        public IEnumerable<KeyValuePair<IEnumerable<RecordType>, TypeGeneration>> GenerationTypes => GetGenerationTypes();
        public bool IsTriggerForObject;
        public RecordTypeConverter RecordTypeConverter;

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
}
