using Loqui.Generation;
using Mutagen.Bethesda.Generation.Modules.Plugin;
using Mutagen.Bethesda.Records;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public static class ObjectGenerationExt
    {
        public static MutagenObjData GetObjectData(this ObjectGeneration objGen)
        {
            return (MutagenObjData)objGen.CustomData.GetOrAdd(Constants.DataKey, () => new MutagenObjData(objGen));
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
                return EnumerableExt<RecordType>.Empty;
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

        public static bool TryGetCustomRecordTypeTriggers(this ObjectGeneration objGen, out IEnumerable<RecordType> recTypes)
        {
            var data = objGen.GetObjectData();
            if (data.CustomRecordTypeTriggers == null
                || data.CustomRecordTypeTriggers.Count == 0)
            {
                recTypes = default;
                return false;
            }
            recTypes = data.CustomRecordTypeTriggers;
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

        public static async Task<bool> IsMajorRecord(this ObjectGeneration objGen)
        {
            if (objGen.GetObjectType() != ObjectType.Record) return false;
            if (objGen.Name == "MajorRecord") return true;
            await Task.WhenAll(objGen.BaseClassTrail().Select(bo => bo.LoadingCompleteTask.Task));
            return objGen.BaseClassTrail().Any(bo => bo.Name == "MajorRecord");
        }

        public static async Task<bool> IsMajorRecord(this LoquiType loqui)
        {
            if (loqui.TargetObjectGeneration != null)
            {
                return await IsMajorRecord(loqui.TargetObjectGeneration);
            }
            else if (loqui.RefType == LoquiType.LoquiRefType.Interface)
            {
                // ToDo  
                // Quick hack.  Real solution should use reflection to investigate the interface  
                return true;
            }
            return false;
        }

        public static bool IsTopLevelGroup(this ObjectGeneration objGen)
        {
            if (objGen.GetObjectType() != ObjectType.Group) return false;
            if (objGen.Name == "Group") return true;
            return false;
        }

        public static ObjectType GetObjectType(this ObjectGeneration objGen)
        {
            var objType = objGen.GetObjectData().ObjectType;
            if (objType.HasValue) return objType.Value;
            return ObjectType.Subrecord;
        }

        public static bool IsListGroup(this ObjectGeneration obj)
        {
            return obj.GetObjectType() == ObjectType.Group
                && obj.Name != "Group";
        }

        public static string GetTriggeringSource(this ObjectGeneration objGen)
        {
            return objGen.GetObjectData().TriggeringSource;
        }

        public static async Task<bool> IsSingleTriggerSource(this ObjectGeneration objGen)
        {
            var enumer = await objGen.GetObjectData().GenerationTypes;
            if (!enumer.SelectMany((e) => e.Key).Distinct().Any()) return false;
            return !enumer.SelectMany((e) => e.Key).Distinct().CountGreaterThan(1);
        }

        public static string RecordTypeHeaderName(this ObjectGeneration objGen, RecordType recType)
        {
            return $"RecordTypes.{recType.CheckedType}";
        }

        public static bool StructNullable(this ObjectGeneration objGen)
        {
            return objGen.IterateFields().Any((f) =>
            {
                if (!f.Nullable) return false;
                var data = f.GetFieldData();
                return !data.HasTrigger;
            });
        }

        public static async Task<LoquiType> GetGroupLoquiType(this ObjectGeneration objGen)
        {
            await objGen.LoadingCompleteTask.Task;
            if (objGen.GetObjectType() != ObjectType.Group)
            {
                throw new ArgumentException();
            }
            foreach (var field in objGen.IterateFields())
            {
                if (field is ContainerType cont)
                {
                    return cont.SubTypeGeneration as LoquiType;
                }
                else if (field is DictType dictType)
                {
                    return dictType.ValueTypeGen as LoquiType;
                }
            }
            throw new ArgumentException();
        }

        public static async Task<LoquiType> GetGroupLoquiTypeLowest(this ObjectGeneration objGen)
        {
            await objGen.LoadingCompleteTask.Task;
            if (objGen.GetObjectType() != ObjectType.Group)
            {
                throw new ArgumentException();
            }

            var loquiType = await GetGroupLoquiType(objGen);
            while (loquiType.TargetObjectGeneration.GetObjectType() == ObjectType.Group)
            {
                loquiType = await GetGroupLoquiType(loquiType.TargetObjectGeneration);
            }
            return loquiType;
        }

        public static ObjectGeneration GetGroupTarget(this LoquiType loqui)
        {
            loqui.TryGetSpecificationAsObject("T", out var ret);
            return ret;
        }

        public static bool IsTypelessStruct(this ObjectGeneration objGen)
        {
            return objGen.GetObjectType() == ObjectType.Subrecord && !objGen.HasRecordType();
        }

        public static bool IsVariableLengthStruct(this ObjectGeneration objGen)
        {
            var objData = objGen.GetObjectData();
            return objGen.GetObjectType() == ObjectType.Subrecord
                && objData.TriggeringSource == null
                && objData.HasVersioning();
        }

        public static async Task<bool> GetNeedsMasters(this ObjectGeneration objGen)
        {
            if (objGen.GetObjectType() == ObjectType.Group) return true;
            foreach (var field in objGen.IterateFields())
            {
                if (field is FormKeyType) return true;
                if (field is FormIDType) return true;
                if (field is ContainerType cont)
                {
                    if (cont.SubTypeGeneration is LoquiType loqui
                        && (loqui.TargetObjectGeneration == null || await loqui.TargetObjectGeneration.GetNeedsMasters()))
                    {
                        return true;
                    }
                }
                if (field is DictType dict)
                {
                    if (dict.ValueTypeGen is LoquiType loqui
                        && (loqui.TargetObjectGeneration == null || await loqui.TargetObjectGeneration.GetNeedsMasters()))
                    {
                        return true;
                    }
                }
            }
            foreach (var baseObj in objGen.BaseClassTrail())
            {
                if (await baseObj.GetNeedsMasters()) return true;
            }
            return false;
        }

        public static bool HasVersionedFields(this ObjectGeneration objGen)
        {
            if (objGen.GetObjectType() != ObjectType.Record) return false;
            return objGen.Fields.Any(f => f.GetFieldData().CustomVersion != null);
        }
    }
}
