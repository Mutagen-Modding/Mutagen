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
        public static MutagenObjData GetObjectData(this ObjectGeneration objGen)
        {
            return (MutagenObjData)objGen.CustomData.TryCreateValue(Constants.DATA_KEY, () => new MutagenObjData(objGen));
        }

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
            var data = objGen.GetObjectData();
            if (data.RecordType == null)
            {
                recType = default;
                return false;
            }
            recType = data.RecordType.Value;
            return true;
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
            var data = objGen.GetObjectData();
            if (data.MarkerType == null)
            {
                recType = default;
                return false;
            }
            recType = data.MarkerType.Value;
            return true;
        }

        public static async Task<TryGet<IEnumerable<RecordType>>> TryGetTriggeringRecordTypes(this ObjectGeneration objGen)
        {
            await objGen.LoadingCompleteTask.Task;
            var data = objGen.GetObjectData();
            return TryGet<IEnumerable<RecordType>>.Create(
                successful: data.TriggeringRecordTypes.Any(),
                val: data.TriggeringRecordTypes);
        }

        public static ObjectType GetObjectType(this ObjectGeneration objGen)
        {
            var objType = objGen.GetObjectData().ObjectType;
            if (objType.HasValue) return objType.Value;
            throw new ArgumentException($"Object {objGen.Name} did not have object type defined.");
        }

        public static string GetTriggeringSource(this ObjectGeneration objGen)
        {
            return objGen.GetObjectData().TriggeringSource;
        }

        public static string RecordTypeHeaderName(this ObjectGeneration objGen, RecordType recType)
        {
            return $"{objGen.RegistrationName}.{recType.Type}_HEADER";
        }
    }
}

