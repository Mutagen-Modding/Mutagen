using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Generation
{
    public class MutagenFieldData
    {
        public RecordType? RecordType;
        public string TriggeringRecordAccessor;
        public bool Optional;
        public long? Length;
        public bool IncludeInLength;
        public bool Vestigial;
        public bool CustomBinary;
    }
}
