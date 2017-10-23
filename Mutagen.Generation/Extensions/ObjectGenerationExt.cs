using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Generation
{
    public static class ObjectGenerationExt
    {
        public static RecordType GetRecordType(this ObjectGeneration objGen)
        {
            if (!TryGetRecordType(objGen, out var data))
            {
                throw new ArgumentException($"Object {objGen.Name} did not have a record type.");
            }
            return data;
        }

        public static bool TryGetRecordType(this ObjectGeneration objGen, out RecordType recType)
        {
            if (objGen.CustomData.TryGetValue(Constants.RECORD_TYPE, out var dataObj))
            {
                recType = (RecordType)dataObj;
                return true;
            }
            recType = default(RecordType);
            return false;
        }

        public static bool HasRecordType(this ObjectGeneration objGen)
        {
            return TryGetRecordType(objGen, out var recType);
        }

        public static RecordType? GetTriggeringRecordType(this ObjectGeneration objGen)
        {
            if (!TryGetTriggeringRecordType(objGen, out var data))
            {
                return null;
            }
            return data;
        }

        public static bool TryGetTriggeringRecordType(this ObjectGeneration objGen, out RecordType recType)
        {
            if (objGen.CustomData.TryGetValue(Constants.TRIGGERING_RECORD_TYPE, out var dataObj))
            {
                recType = (RecordType)dataObj;
                return true;
            }
            recType = default(RecordType);
            return false;
        }

        public static ObjectType GetObjectType(this ObjectGeneration objGen)
        {
            if (objGen.CustomData.TryGetValue(Constants.OBJECT_TYPE, out var dataObj))
            {
                return (ObjectType)dataObj;
            }
            throw new ArgumentException($"Object {objGen.Name} did not have object type defined.");
        }
    }
}
