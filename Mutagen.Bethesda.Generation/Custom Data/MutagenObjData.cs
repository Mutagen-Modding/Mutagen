using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class MutagenObjData
    {
        public RecordType? RecordType;
        public bool FailOnUnknown;
        public ObjectType? ObjectType;
        public RecordType? MarkerType;
        public HashSet<RecordType> TriggeringRecordTypes = new HashSet<RecordType>();
        public string TriggeringSource;
        public TaskCompletionSource<bool> TCS = new TaskCompletionSource<bool>();
    }
}
