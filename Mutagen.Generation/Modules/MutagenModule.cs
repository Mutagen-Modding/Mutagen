using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using System.Xml.Linq;

namespace Mutagen.Generation
{
    public class MutagenModule : GenerationModule
    {
        public override string RegionString => "Mutagen";

        public override void GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            GenerateGenericRecordTypes(obj, fg);
        }

        public override void GenerateInRegistration(ObjectGeneration obj, FileGeneration fg)
        {
            GenerateKnownRecordTypes(obj, fg);
            fg.AppendLine($"public const int NumStructFields = {obj.Fields.Where((f) => f.GetFieldData().TriggeringRecordAccessor == null).Count()};");
            fg.AppendLine($"public const int NumTypedFields = {obj.Fields.Where((f) => f.GetFieldData().TriggeringRecordAccessor != null).Count()};");
        }

        private IEnumerable<string> GetGenerics(ObjectGeneration obj, FileGeneration fg)
        {
            HashSet<string> genericNames = new HashSet<string>();
            foreach (var field in obj.Fields)
            {
                if (!(field is LoquiType loquiType))
                {
                    if ((field is ContainerType container)
                        && container.SubTypeGeneration is LoquiType contLoquiType)
                    {
                        loquiType = contLoquiType;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (loquiType.GenericDef == null) continue;
                genericNames.Add(loquiType.GenericDef.Name);
            }
            return genericNames;
        }

        private void GenerateKnownRecordTypes(ObjectGeneration obj, FileGeneration fg)
        {
            HashSet<RecordType> recordTypes = new HashSet<RecordType>();
            RecordType? triggeringRecType = null;
            if (obj.TryGetRecordType(out var recType))
            {
                recordTypes.Add(recType);
            }
            if (obj.TryGetTriggeringRecordType(out recType))
            {
                triggeringRecType = recType;
                recordTypes.Add(recType);
            }
            foreach (var field in obj.Fields)
            {
                var data = field.GetFieldData();
                if (data.RecordType.HasValue)
                {
                    recordTypes.Add(data.RecordType.Value);
                }
                if (data.TriggeringRecordType.HasValue)
                {
                    recordTypes.Add(data.TriggeringRecordType.Value);
                }
                if (data.MarkerType.HasValue)
                {
                    recordTypes.Add(data.MarkerType.Value);
                }
                if (field is ContainerType contType)
                {
                    if (!contType.SubTypeGeneration.TryGetFieldData(out var subData)) continue;
                    if (!subData.TriggeringRecordType.HasValue) continue;
                    recordTypes.Add(subData.TriggeringRecordType.Value);
                }
            }
            foreach (var type in recordTypes)
            {
                fg.AppendLine($"public static readonly {nameof(RecordType)} {type.HeaderName} = new {nameof(RecordType)}(\"{type.Type}\");");
            }
            if (triggeringRecType.HasValue)
            {
                fg.AppendLine($"public static readonly {nameof(RecordType)} {Mutagen.Constants.TRIGGERING_RECORDTYPE_MEMBER} = {triggeringRecType.Value.HeaderName};");
            }
        }

        public override void GenerateInStaticCtor(ObjectGeneration obj, FileGeneration fg)
        {
            foreach (var genName in GetGenerics(obj, fg))
            {
                fg.AppendLine($"var register = LoquiRegistration.GetRegister(typeof(T));");
                fg.AppendLine($"{genName}_RecordType = (RecordType)register.GetType().GetField(\"{Mutagen.Constants.TRIGGERING_RECORDTYPE_MEMBER}\").GetValue(null);");
            }
        }

        private void GenerateGenericRecordTypes(ObjectGeneration obj, FileGeneration fg)
        {
            foreach (var genName in GetGenerics(obj, fg))
            {
                fg.AppendLine($"public static readonly {nameof(RecordType)} {genName}_RecordType;");
            }
        }

        public override void PreLoad(ObjectGeneration obj)
        {
            var record = obj.Node.GetAttribute("recordType");
            var isGRUP = obj.Name.Equals("Group");
            if (record != null && !isGRUP)
            {
                obj.CustomData[Constants.RECORD_TYPE] = new RecordType(record);
                obj.CustomData[Constants.TRIGGERING_RECORD_TYPE] = new RecordType(record);
            }
            else
            {
                var field = obj.Node.Element(XName.Get("Fields", LoquiGenerator.Namespace))?.Elements().FirstOrDefault();
                if (field != null)
                {
                    record = field.GetAttribute("recordType");
                    if (record != null)
                    {
                        obj.CustomData[Constants.TRIGGERING_RECORD_TYPE] = new RecordType(record);
                    }
                }
            }
            obj.CustomData[Constants.FAIL_ON_UNKNOWN] = obj.Node.GetAttribute<bool>("failOnUnknownType", defaultVal: false);

            if (isGRUP)
            {
                obj.CustomData[Constants.RECORD_TYPE] = new RecordType("GRUP");
                obj.CustomData[Constants.TRIGGERING_RECORD_TYPE] = new RecordType("GRUP");
            }

            var objType = obj.Node.GetAttribute("objType");
            if (!Enum.TryParse<ObjectType>(objType, out var objTypeEnum))
            {
                throw new ArgumentException("Must specify object type.");
            }
            obj.CustomData[Constants.OBJECT_TYPE] = objTypeEnum;

            if (obj.Node.TryGetAttribute("markerType", out var markerType))
            {
                var markerTypeRec = new RecordType(markerType.Value);
                obj.CustomData[Constants.MARKER_TYPE] = markerTypeRec;
                obj.CustomData[Constants.RECORD_TYPE] = markerTypeRec;
                obj.CustomData[Constants.TRIGGERING_RECORD_TYPE] = markerTypeRec;
            }
        }

        public override void PostFieldLoad(ObjectGeneration obj, TypeGeneration field, XElement node)
        {
            var data = field.CustomData.TryCreateValue(Constants.DATA_KEY, () => new MutagenFieldData()) as MutagenFieldData;
            var recordAttr = node.GetAttribute("recordType");
            if (recordAttr != null)
            {
                data.RecordType = new RecordType(recordAttr);
            }
            var markerAttr = node.GetAttribute("markerType");
            if (markerAttr != null)
            {
                data.MarkerType = new RecordType(markerAttr);
            }
            if (recordAttr != null
                && markerAttr != null)
            {
                throw new ArgumentException("Cannot have both record type and marker type defined");
            }
            SetRecordTrigger(obj, field, data);
            ModifyGRUPRecordTrigger(obj, field, data);

            data.Optional = node.GetAttribute<bool>("optional", false);
            if (data.Optional && !data.RecordType.HasValue)
            {
                throw new ArgumentException("Cannot have an optional field if it is not a record typed field.");
            }
            data.Length = node.GetAttribute<long?>("length", null);
            if (field is ByteArrayType byteArray
                && !data.Length.HasValue)
            {
                data.Length = 4;
            }
            if (!data.Length.HasValue
                && !data.RecordType.HasValue
                && !(field is PrimitiveType)
                && !(field is ContainerType))
            {
                throw new ArgumentException("Have to define either length or record type.");
            }
            data.IncludeInLength = node.GetAttribute<bool>("includeInLength", true);
            data.Vestigial = node.GetAttribute<bool>("vestigial", false);
            data.CustomBinary = node.GetAttribute<bool>("customBinary", false);
        }

        public override void PostLoad(ObjectGeneration obj)
        {
            base.PostLoad(obj);
            Dictionary<string, TypeGeneration> triggerMapping = new Dictionary<string, TypeGeneration>();
            foreach (var field in obj.Fields)
            {
                if (!field.TryGetFieldData(out var mutaData)) continue;
                if (mutaData.TriggeringRecordAccessor == null) continue;
                if (triggerMapping.TryGetValue(mutaData.TriggeringRecordAccessor, out var existingField))
                {
                    throw new ArgumentException($"{obj.Name} cannot have two fields that have the same trigger {mutaData.TriggeringRecordAccessor}: {existingField.Name} AND {field.Name}");
                }
                triggerMapping[mutaData.TriggeringRecordAccessor] = field;
            }
        }

        private void SetRecordTrigger(
            ObjectGeneration obj,
            TypeGeneration field,
            MutagenFieldData data)
        {
            RecordType recType;
            if (field is LoquiType loqui)
            {
                if (loqui.RefGen != null
                    && loqui.RefGen.Obj.TryGetTriggeringRecordType(out recType))
                {
                    if (loqui.RefGen.Name.Equals("Group"))
                    {
                        var objName = loqui.GenericSpecification.Specifications["T"];
                        var grupObj = obj.ProtoGen.ObjectGenerationsByName[objName];
                        data.RecordType = grupObj.GetRecordType();
                        data.TriggeringRecordAccessor = $"{grupObj.RegistrationName}.{data.RecordType.Value.HeaderName}";
                        data.TriggeringRecordType = data.RecordType;
                    }
                    else
                    {
                        data.RecordType = recType;
                    }
                }
                else if (loqui.GenericDef != null)
                {
                    data.TriggeringRecordAccessor = $"{loqui.GenericDef.Name}_RecordType";
                }
            }
            else if (field is LoquiListType loquiList
                && !data.RecordType.HasValue)
            {
                loqui = loquiList.SubTypeGeneration as LoquiType;
                if (loqui.RefGen.Obj.TryGetTriggeringRecordType(out recType))
                {
                    data.TriggeringRecordType = recType;
                }
                else if (loqui.GenericDef != null)
                {
                    data.TriggeringRecordAccessor = $"{loqui.GenericDef.Name}_RecordType";
                }
            }
            else if (field is ListType listType
                && !data.RecordType.HasValue)
            {
                if (listType.SubTypeGeneration is LoquiType subListLoqui)
                {
                    if (subListLoqui.RefGen != null
                        && subListLoqui.RefGen.Obj.TryGetTriggeringRecordType(out recType))
                    {
                        data.TriggeringRecordType = recType;
                    }
                    else if (subListLoqui.GenericDef != null)
                    {
                        data.TriggeringRecordAccessor = $"{subListLoqui.GenericDef.Name}_RecordType";
                    }
                }
                else
                {
                    var subData = listType.SubTypeGeneration.CustomData.TryCreateValue(Constants.DATA_KEY, () => new MutagenFieldData()) as MutagenFieldData;
                    if (subData.TriggeringRecordAccessor != null)
                    {
                        data.TriggeringRecordAccessor = $"{obj.RegistrationName}.{subData.RecordType.Value.HeaderName}";
                        data.TriggeringRecordType = subData.RecordType;
                        data.RecordType = subData.RecordType;
                    }
                }
            }

            SetTriggeringRecordAccessors(obj, data);

            if (field is ContainerType contType
                && contType.SubTypeGeneration is LoquiType contLoqui)
            {
                var subData = contLoqui.CustomData.TryCreateValue(Constants.DATA_KEY, () => new MutagenFieldData()) as MutagenFieldData;
                SetRecordTrigger(
                    obj,
                    contLoqui,
                    subData);
            }
        }

        private void SetTriggeringRecordAccessors(ObjectGeneration obj, MutagenFieldData data)
        {
            if (data.TriggeringRecordAccessor != null) return;
            if (data.MarkerType.HasValue)
            {
                data.TriggeringRecordAccessor = $"{obj.RegistrationName}.{data.MarkerType.Value.HeaderName}";
                data.TriggeringRecordType = data.MarkerType;
            }
            else if (data.RecordType.HasValue)
            {
                data.TriggeringRecordAccessor = $"{obj.RegistrationName}.{data.RecordType.Value.HeaderName}";
                data.TriggeringRecordType = data.RecordType;
            }
            else if (data.TriggeringRecordType.HasValue)
            {
                data.TriggeringRecordAccessor = $"{obj.RegistrationName}.{data.TriggeringRecordType.Value.HeaderName}";
            }
        }

        private void ModifyGRUPRecordTrigger(
            ObjectGeneration obj,
            TypeGeneration field,
            MutagenFieldData data)
        {
            if (obj.Name.Equals("Group") && (field.Name?.Equals("Items") ?? false))
            {
                ListType list = field as ListType;
                LoquiType loqui = list.SubTypeGeneration as LoquiType;
                data.TriggeringRecordAccessor = $"Group<T>.T_RecordType";
            }
        }
    }
}
