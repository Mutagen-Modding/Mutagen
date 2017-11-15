using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Generation
{
    public class MutagenFieldData
    {
        public readonly TypeGeneration SourceTypeGeneration;
        public RecordType? MarkerType { get; set; }
        public RecordType? RecordType { get; set; }
        public RecordType? TriggeringRecordType { get; set; }
        public string TriggeringRecordAccessor;
        public bool HasTrigger => !string.IsNullOrWhiteSpace(this.TriggeringRecordAccessor);
        public bool Optional;
        public long? Length;
        public bool IncludeInLength;
        public bool Vestigial;
        public bool CustomBinary;
        public Dictionary<RecordType, ObjectGeneration> SubLoquiTypes = new Dictionary<RecordType, ObjectGeneration>();
        public IEnumerable<KeyValuePair<RecordType, TypeGeneration>> GenerationTypes => GetGenerationTypes();

        public MutagenFieldData(TypeGeneration source)
        {
            this.SourceTypeGeneration = source;
        }

        private IEnumerable<KeyValuePair<RecordType, TypeGeneration>> GetGenerationTypes()
        {
            yield return new KeyValuePair<Mutagen.RecordType, TypeGeneration>(
                this.TriggeringRecordType.Value,
                this.SourceTypeGeneration);
            if (!(this.SourceTypeGeneration is LoquiType loqui)) yield break;
            foreach (var subType in this.SubLoquiTypes)
            {
                yield return new KeyValuePair<Mutagen.RecordType, TypeGeneration>(
                    subType.Key,
                    loqui.Spawn(subType.Value));
            }
        }
    }
}
