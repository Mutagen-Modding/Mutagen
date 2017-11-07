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

        public void CopyOver(MutagenFieldData other)
        {
            this.RecordType = other.RecordType;
            this.TriggeringRecordType = other.TriggeringRecordType;
            this.TriggeringRecordAccessor = other.TriggeringRecordAccessor;
        }
    }
}
