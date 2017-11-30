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

        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            GenerateGenericRecordTypes(obj, fg);
        }

        public override async Task GenerateInRegistration(ObjectGeneration obj, FileGeneration fg)
        {
            GenerateKnownRecordTypes(obj, fg);
            fg.AppendLine($"public const int NumStructFields = {obj.IterateFields(expandSets: SetMarkerType.ExpandSets.False).Where((f) => !f.GetFieldData().HasTrigger).Count()};");
            var typedFields = obj.IterateFields().Where((f) => f.GetFieldData().HasTrigger).Sum((f) =>
            {
                if (!(f is SetMarkerType set)) return 1;
                return set.IterateFields().Count();
            });
            fg.AppendLine($"public const int NumTypedFields = {typedFields};");
        }

        private IEnumerable<string> GetGenerics(ObjectGeneration obj, FileGeneration fg)
        {
            HashSet<string> genericNames = new HashSet<string>();
            foreach (var field in obj.IterateFields())
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
            if (obj.TryGetRecordType(out var recType))
            {
                recordTypes.Add(recType);
            }
            if (obj.TryGetTriggeringRecordTypes(out var triggeringRecType))
            {
                recordTypes.Add(triggeringRecType);
            }
            foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.FalseAndInclude, nonIntegrated: true))
            {
                var data = field.GetFieldData();
                if (data.RecordType.HasValue)
                {
                    recordTypes.Add(data.RecordType.Value);
                }
                recordTypes.Add(data.TriggeringRecordTypes);
                if (data.MarkerType.HasValue)
                {
                    recordTypes.Add(data.MarkerType.Value);
                }
                foreach (var subType in data.SubLoquiTypes)
                {
                    recordTypes.Add(subType.Key);
                }
                if (field is ContainerType contType)
                {
                    if (!contType.SubTypeGeneration.TryGetFieldData(out var subData)) continue;
                    if (!subData.HasTrigger) continue;
                    recordTypes.Add(subData.TriggeringRecordTypes);
                }
            }
            foreach (var type in recordTypes)
            {
                fg.AppendLine($"public static readonly {nameof(RecordType)} {type.Type}_HEADER = new {nameof(RecordType)}(\"{type.Type}\");");
            }
            var count = triggeringRecType.Count();
            if (count == 1)
            {
                fg.AppendLine($"public static readonly {nameof(RecordType)} {Mutagen.Constants.TRIGGERING_RECORDTYPE_MEMBER} = {triggeringRecType.First().Type}_HEADER;");
            }
            else if (count > 1)
            {
                fg.AppendLine($"public static IEnumerable<RecordType> TriggeringRecordTypes => _TriggeringRecordTypes.Value;");
                fg.AppendLine($"private static readonly Lazy<HashSet<RecordType>> _TriggeringRecordTypes = new Lazy<HashSet<RecordType>>(() =>");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"return new HashSet<RecordType>(");
                    using (new DepthWrapper(fg))
                    {
                        fg.AppendLine($"new RecordType[]");
                        using (new BraceWrapper(fg))
                        {
                            foreach (var trigger in triggeringRecType)
                            {
                                fg.AppendLine($"{trigger.Type}_HEADER");
                            }
                        }
                    }
                }
            }
        }

        public override async Task GenerateInStaticCtor(ObjectGeneration obj, FileGeneration fg)
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

        public override async Task PreLoad(ObjectGeneration obj)
        {
            var record = obj.Node.GetAttribute("recordType");
            var isGRUP = obj.Name.Equals("Group");
            if (record != null && !isGRUP)
            {
                obj.CustomData[Constants.RECORD_TYPE] = new RecordType(record);
                obj.CustomData[Constants.TRIGGERING_RECORD_TYPE] = new RecordType[] { new RecordType(record) };
            }
            else
            {
                var field = obj.Node.Element(XName.Get("Fields", LoquiGenerator.Namespace))?.Elements().FirstOrDefault();
                if (field != null)
                {
                    record = field.GetAttribute("recordType");
                    if (record != null)
                    {
                        obj.CustomData[Constants.TRIGGERING_RECORD_TYPE] = new RecordType[] { new RecordType(record) };
                    }
                }
            }
            obj.CustomData[Constants.FAIL_ON_UNKNOWN] = obj.Node.GetAttribute<bool>("failOnUnknownType", defaultVal: false);

            if (isGRUP)
            {
                obj.CustomData[Constants.RECORD_TYPE] = new RecordType("GRUP");
                obj.CustomData[Constants.TRIGGERING_RECORD_TYPE] = new RecordType[] { new RecordType("GRUP") };
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
                obj.CustomData[Constants.TRIGGERING_RECORD_TYPE] = new RecordType[] { markerTypeRec };
            }

            if (obj.CustomData.TryGetValue(Constants.TRIGGERING_RECORD_TYPE, out var trigRecTypeObj))
            {
                var trigRecTypes = trigRecTypeObj as IEnumerable<RecordType>;
                if (trigRecTypes.CountGreaterThan(1))
                {
                    throw new NotImplementedException();
                }
                else
                {
                    obj.CustomData[Constants.TRIGGERING_SOURCE] = obj.RecordTypeHeaderName(trigRecTypes.First());
                }
            }
        }

        public override async Task PostFieldLoad(ObjectGeneration obj, TypeGeneration field, XElement node)
        {
            var data = field.CustomData.TryCreateValue(Constants.DATA_KEY, () => new MutagenFieldData(field)) as MutagenFieldData;
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

        public override async Task PostLoad(ObjectGeneration obj)
        {
            foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.TrueAndInclude))
            {
                SetRecordTrigger(obj, field, field.GetFieldData());
            }
            await base.PostLoad(obj);
            Dictionary<string, TypeGeneration> triggerMapping = new Dictionary<string, TypeGeneration>();
            foreach (var field in obj.IterateFields())
            {
                if (!field.TryGetFieldData(out var mutaData)) continue;
                if (!mutaData.HasTrigger) continue;
                foreach (var trigger in mutaData.TriggeringRecordAccessors)
                {
                    if (triggerMapping.TryGetValue(trigger, out var existingField))
                    {
                        throw new ArgumentException($"{obj.Name} cannot have two fields that have the same trigger {trigger}: {existingField.Name} AND {field.Name}");
                    }
                    triggerMapping[trigger] = field;
                }
            }
        }

        public override async Task Resolve(ObjectGeneration obj)
        {
            foreach (var field in obj.IterateFields())
            {
                if (!(field is LoquiType loqui)) continue;
                if (loqui.TargetObjectGeneration == null) continue;
                var inheritingObjs = await loqui.TargetObjectGeneration.InheritingObjects();
                var data = loqui.GetFieldData();
                foreach (var subObj in inheritingObjs)
                {
                    if (!subObj.TryGetTriggeringRecordTypes(out var subRecs)) continue;
                    foreach (var subRec in subRecs)
                    {
                        data.SubLoquiTypes.Add(subRec, subObj);
                    }
                }
            }
        }

        private void SetRecordTrigger(
            ObjectGeneration obj,
            TypeGeneration field,
            MutagenFieldData data)
        {
            RecordType recType;
            IEnumerable<RecordType> trigRecTypes;
            if (field is LoquiType loqui)
            {
                if (loqui.TargetObjectGeneration != null
                    && (loqui.TargetObjectGeneration.TryGetTriggeringRecordTypes(out trigRecTypes)
                        | loqui.TargetObjectGeneration.TryGetRecordType(out recType)))
                {
                    if (loqui.TargetObjectGeneration.Name.Equals("Group"))
                    {
                        var objName = loqui.GenericSpecification.Specifications["T"];
                        var grupObj = obj.ProtoGen.ObjectGenerationsByName[objName];
                        data.RecordType = grupObj.GetRecordType();
                        data.TriggeringRecordAccessors.Add(grupObj.RecordTypeHeaderName(data.RecordType.Value));
                        data.TriggeringRecordTypes.Add(data.RecordType.Value);
                    }
                    else
                    {
                        if (loqui.TargetObjectGeneration.TryGetRecordType(out recType))
                        {
                            data.RecordType = recType;
                        }
                        data.TriggeringRecordTypes.Add(trigRecTypes);
                    }
                }
                else if (loqui.GenericDef != null)
                {
                    data.TriggeringRecordAccessors.Add($"{loqui.GenericDef.Name}_RecordType");
                }
            }
            else if (field is LoquiListType loquiList
                && !data.RecordType.HasValue)
            {
                loqui = loquiList.SubTypeGeneration as LoquiType;
                if (loqui.TargetObjectGeneration.TryGetTriggeringRecordTypes(out trigRecTypes))
                {
                    data.TriggeringRecordTypes.Add(trigRecTypes);
                }
                else if (loqui.GenericDef != null)
                {
                    data.TriggeringRecordAccessors.Add($"{loqui.GenericDef.Name}_RecordType");
                }
            }
            else if (field is ListType listType
                && !data.RecordType.HasValue)
            {
                if (listType.SubTypeGeneration is LoquiType subListLoqui)
                {
                    if (subListLoqui.TargetObjectGeneration != null
                        && subListLoqui.TargetObjectGeneration.TryGetTriggeringRecordTypes(out trigRecTypes))
                    {
                        data.TriggeringRecordTypes.Add(trigRecTypes);
                    }
                    else if (subListLoqui.GenericDef != null)
                    {
                        data.TriggeringRecordAccessors.Add($"{subListLoqui.GenericDef.Name}_RecordType");
                    }
                }
                else
                {
                    var subData = listType.SubTypeGeneration.CustomData.TryCreateValue(Constants.DATA_KEY, () => new MutagenFieldData(listType.SubTypeGeneration)) as MutagenFieldData;
                    SetRecordTrigger(obj, listType.SubTypeGeneration, subData);
                    if (subData.HasTrigger)
                    {
                        data.TriggeringRecordAccessors.Add(obj.RecordTypeHeaderName(subData.RecordType.Value));
                        data.TriggeringRecordTypes.Add(subData.RecordType.Value);
                        data.RecordType = subData.RecordType;
                    }
                }
            }

            SetTriggeringRecordAccessors(obj, field, data);

            if (field is ContainerType contType
                && contType.SubTypeGeneration is LoquiType contLoqui)
            {
                var subData = contLoqui.CustomData.TryCreateValue(Constants.DATA_KEY, () => new MutagenFieldData(contLoqui)) as MutagenFieldData;
                SetRecordTrigger(
                    obj,
                    contLoqui,
                    subData);
            }
        }

        private void SetTriggeringRecordAccessors(ObjectGeneration obj, TypeGeneration field, MutagenFieldData data)
        {
            if (!data.HasTrigger)
            {
                if (data.MarkerType.HasValue)
                {
                    data.TriggeringRecordAccessors.Add(obj.RecordTypeHeaderName(data.MarkerType.Value));
                    data.TriggeringRecordTypes.Add(data.MarkerType.Value);
                }
                else if (data.RecordType.HasValue)
                {
                    data.TriggeringRecordAccessors.Add(obj.RecordTypeHeaderName(data.RecordType.Value));
                    data.TriggeringRecordTypes.Add(data.RecordType.Value);
                }
                else if (data.TriggeringRecordTypes.Count > 0)
                {
                    foreach (var trigger in data.TriggeringRecordTypes)
                    {
                        data.TriggeringRecordAccessors.Add(obj.RecordTypeHeaderName(trigger));
                    }
                }
            }
            if (data.RecordType.HasValue)
            {
                data.TriggeringRecordSetAccessor = obj.RecordTypeHeaderName(data.RecordType.Value);
            }
            else if (data.TriggeringRecordTypes.Count== 1)
            {
                data.TriggeringRecordSetAccessor = obj.RecordTypeHeaderName(data.TriggeringRecordTypes.First());
            }
            else if (field is LoquiType loqui)
            {
                if (loqui.TargetObjectGeneration != null)
                {
                    data.TriggeringRecordSetAccessor = $"{loqui.TargetObjectGeneration.RegistrationName}.TriggeringRecordTypes";
                }
                else if (data.TriggeringRecordAccessors.Count == 1)
                {
                    data.TriggeringRecordSetAccessor = data.TriggeringRecordAccessors.First();
                }
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
                data.TriggeringRecordAccessors.Add($"T_RecordType");
            }
        }
    }
}
