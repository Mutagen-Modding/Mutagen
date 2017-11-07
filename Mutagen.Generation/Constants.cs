using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Generation
{
    public class Constants
    {
        public readonly static string DATA_KEY = "MutagenData";
        public readonly static string RECORD_TYPE = nameof(RecordType);
        public readonly static string MARKER_TYPE = "MarkerType";
        public readonly static string OBJECT_TYPE = nameof(ObjectType);
        public readonly static string TRIGGERING_RECORD_TYPE = $"Triggering{nameof(RecordType)}";
        public readonly static string FAIL_ON_UNKNOWN = $"FailOnUnknown";
    }
}
