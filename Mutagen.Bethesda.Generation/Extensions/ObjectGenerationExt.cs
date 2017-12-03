using Loqui.Generation;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
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

        public static async Task<IEnumerable<RecordType>> GetTriggeringRecordTypes(this ObjectGeneration objGen)
        {
            var data = await TryGetTriggeringRecordTypes(objGen);
            if (data.Failed)
            {
                return EnumerableExt<RecordType>.EMPTY;
            }
            return data.Value;
        }

        public static bool TryGetMarkerType(this ObjectGeneration objGen, out RecordType recType)
        {
            if (objGen.CustomData.TryGetValue(Constants.MARKER_TYPE, out var dataObj))
            {
                recType = (RecordType)dataObj;
                return true;
            }
            recType = default(RecordType);
            return false;
        }

        public static async Task<TryGet<IEnumerable<RecordType>>> TryGetTriggeringRecordTypes(this ObjectGeneration objGen)
        {
            if (objGen.CustomData.TryGetValue(Constants.TRIGGERING_RECORD_TYPE, out var dataObj))
            {
                var enumer = (IEnumerable<RecordType>)dataObj;
                return TryGet<IEnumerable<RecordType>>.Create(
                    successful: enumer.Any(),
                    val: enumer);
            }
            var taskObj = (TaskCompletionSource<bool>)objGen.CustomData.TryCreateValue(Constants.TRIGGERING_RECORD_TASK, () => new TaskCompletionSource<bool>());
            await taskObj.Task;
            return await TryGetTriggeringRecordTypes(objGen);
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

