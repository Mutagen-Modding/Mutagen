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

        public static IEnumerable<RecordType> GetTriggeringRecordTypes(this ObjectGeneration objGen)
        {
            if (!TryGetTriggeringRecordTypes(objGen, out var data))
            {
                return EnumerableExt<RecordType>.EMPTY;
            }
            return data;
        }

        public static bool TryGetTriggeringRecordTypes(this ObjectGeneration objGen, out IEnumerable<RecordType> recTypes)
        {
            if (objGen.CustomData.TryGetValue(Constants.TRIGGERING_RECORD_TYPE, out var dataObj))
            {
                recTypes = (IEnumerable<RecordType>)dataObj;
                return true;
            }
            recTypes = EnumerableExt<RecordType>.EMPTY;
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

        public static string GetTriggeringSource(this ObjectGeneration objGen)
        {
            return objGen.CustomData[Constants.TRIGGERING_SOURCE] as string;
        }

        public static string RecordTypeHeaderName(this ObjectGeneration objGen, RecordType recType)
        {
            return $"{objGen.RegistrationName}.{recType.Type}_HEADER";
    }
    }
}
