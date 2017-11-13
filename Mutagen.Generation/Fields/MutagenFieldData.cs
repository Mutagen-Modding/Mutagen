using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Generation
{
    public class MutagenFieldData
    {
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
        public List<RecordType> SubTypes = new List<RecordType>();
        public IEnumerable<RecordType> TriggeringRecordTypes => TriggeringRecordType.Value.And(SubTypes);
    }
}
