using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using System.Xml.Linq;
using Noggog;
using Mutagen.Bethesda.Binary;

namespace Mutagen.Bethesda.Generation
{
    public class MutagenModule : GenerationModule
    {
        public override string RegionString => "Mutagen";

        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            GenerateGenericRecordTypes(obj, fg);
            GenerateSpecialParses(obj, fg);
        }

        public override async Task GenerateInRegistration(ObjectGeneration obj, FileGeneration fg)
        {
            await GenerateKnownRecordTypes(obj, fg);
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

        private async Task GenerateKnownRecordTypes(ObjectGeneration obj, FileGeneration fg)
        {
            HashSet<RecordType> recordTypes = new HashSet<RecordType>();
            if (obj.TryGetRecordType(out var recType))
            {
                recordTypes.Add(recType);
            }
            var trigRecType = await obj.TryGetTriggeringRecordTypes();
            if (trigRecType.Succeeded)
            {
                recordTypes.Add(trigRecType.Value);
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
            var count = trigRecType.Value.Count();
            if (count == 1)
            {
                fg.AppendLine($"public static readonly {nameof(RecordType)} {Mutagen.Bethesda.Constants.TRIGGERING_RECORDTYPE_MEMBER} = {trigRecType.Value.First().Type}_HEADER;");
            }
            else if (count > 1)
            {
                fg.AppendLine($"public static ICollectionGetter<RecordType> TriggeringRecordTypes => _TriggeringRecordTypes.Value;");
                fg.AppendLine($"private static readonly Lazy<ICollectionGetter<RecordType>> _TriggeringRecordTypes = new Lazy<ICollectionGetter<RecordType>>(() =>");
                using (new BraceWrapper(fg) { AppendSemicolon = true,  AppendParenthesis = true })
                {
                    fg.AppendLine($"return new CollectionGetterWrapper<RecordType>(");
                    using (new DepthWrapper(fg))
                    {
                        fg.AppendLine("new HashSet<RecordType>(");
                        using (new DepthWrapper(fg))
                        {
                            fg.AppendLine($"new RecordType[]");
                            using (new BraceWrapper(fg) { AppendParenthesis = true })
                            {
                                using (var comma = new CommaWrapper(fg))
                                {
                                    foreach (var trigger in trigRecType.Value)
                                    {
                                        comma.Add($"{trigger.Type}_HEADER");
                                    }
                                }
                            }
                        }
                    }
                    fg.AppendLine(");");
                }
            }
        }

        public override async Task GenerateInStaticCtor(ObjectGeneration obj, FileGeneration fg)
        {
            foreach (var genName in GetGenerics(obj, fg))
            {
                fg.AppendLine($"var register = LoquiRegistration.GetRegister(typeof(T));");
                fg.AppendLine($"{genName}_RecordType = (RecordType)register.GetType().GetField(\"{Mutagen.Bethesda.Constants.TRIGGERING_RECORDTYPE_MEMBER}\").GetValue(null);");
            }
        }

        private void GenerateGenericRecordTypes(ObjectGeneration obj, FileGeneration fg)
        {
            foreach (var genName in GetGenerics(obj, fg))
            {
                fg.AppendLine($"public static readonly {nameof(RecordType)} {genName}_RecordType;");
            }
        }

        private void GenerateSpecialParses(ObjectGeneration obj, FileGeneration fg)
        {
            foreach (var field in obj.IterateFields(nonIntegrated: true, expandSets: SetMarkerType.ExpandSets.True))
            {
                if (!(field is SpecialParseType special)) continue;
                using (var args = new ArgsWrapper(fg,
                    $"static partial void SpecialParse_{field.Name}"))
                {
                    args.Add($"{obj.ObjectName} item");
                    args.Add($"{nameof(MutagenFrame)} frame");
                    args.Add($"Func<{obj.Mask(MaskType.Error)}> errorMask");
                }
                using (var args = new ArgsWrapper(fg,
                    $"static partial void SpecialWrite_{field.Name}"))
                {
                    args.Add($"{obj.Getter_InterfaceStr} item");
                    args.Add($"{nameof(MutagenWriter)} writer");
                    args.Add($"Func<{obj.Mask(MaskType.Error)}> errorMask");
                }
                using (var args = new FunctionWrapper(fg,
                    $"internal static void SpecialWrite_{field.Name}_Internal"))
                {
                    args.Add($"{obj.Getter_InterfaceStr} item");
                    args.Add($"{nameof(MutagenWriter)} writer");
                    args.Add($"Func<{obj.Mask(MaskType.Error)}> errorMask");
                }
                using (new BraceWrapper(fg))
                {
                    using (var args = new ArgsWrapper(fg,
                        $"SpecialWrite_{field.Name}"))
                    {
                        args.Add($"item: item");
                        args.Add($"writer: writer");
                        args.Add($"errorMask: errorMask");
                    }
                }
            }
        }

        public override async Task PreLoad(ObjectGeneration obj)
        {
            var data = obj.GetObjectData();
            var record = obj.Node.GetAttribute("recordType");
            var isGRUP = obj.Name.Equals("Group");
            if (record != null && !isGRUP)
            {
                data.RecordType = new RecordType(record);
            }
            data.FailOnUnknown = obj.Node.GetAttribute<bool>("failOnUnknownType", defaultVal: false);

            if (isGRUP)
            {
                data.RecordType = new RecordType("GRUP");
            }

            var objType = obj.Node.GetAttribute("objType");
            if (!Enum.TryParse<ObjectType>(objType, out var objTypeEnum))
            {
                throw new ArgumentException("Must specify object type.");
            }
            data.ObjectType = objTypeEnum;

            if (obj.Node.TryGetAttribute("markerType", out var markerType))
            {
                var markerTypeRec = new RecordType(markerType.Value);
                data.MarkerType = markerTypeRec;
                data.RecordType = markerTypeRec;
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
                throw new ArgumentException($"{obj.Name} {field.Name} cannot have both record type and marker type defined");
            }
            ModifyGRUPRecordTrigger(obj, field, data);

            data.Optional = node.GetAttribute<bool>("optional", false);
            if (data.Optional && !data.RecordType.HasValue)
            {
                throw new ArgumentException($"{obj.Name} {field.Name} cannot have an optional field if it is not a record typed field.");
            }
            data.Length = node.GetAttribute<long?>("length", null);
            if (field is ByteArrayType byteArray
                && !data.Length.HasValue)
            {
                data.Length = 4;
            }
            if (!data.Length.HasValue
                && !data.RecordType.HasValue
                && !(field is NothingType)
                && !(field is PrimitiveType)
                && !(field is ContainerType))
            {
                throw new ArgumentException($"{obj.Name} {field.Name} have to define either length or record type.");
            }
            data.IncludeInLength = node.GetAttribute<bool>("includeInLength", true);
            data.Vestigial = node.GetAttribute<bool>("vestigial", false);
            data.CustomBinary = node.GetAttribute<bool>("customBinary", false);
        }

        private async Task SetObjectTrigger(ObjectGeneration obj)
        {
            var data = obj.GetObjectData();
            var isGRUP = obj.Name.Equals("Group");
            if (obj.TryGetRecordType(out var recType) && !isGRUP)
            {
                data.TriggeringRecordTypes.Add(recType);
            }
            else
            {
                HashSet<RecordType> recTypes = new HashSet<RecordType>();
                foreach (var field in obj.IterateFields(
                    nonIntegrated: false,
                    expandSets: SetMarkerType.ExpandSets.FalseAndInclude))
                {
                    if (!field.TryGetFieldData(out var fieldData)) break;
                    if (!fieldData.HasTrigger) break;
                    recTypes.Add(fieldData.TriggeringRecordTypes);
                    fieldData.IsTriggerForObject = true;
                    if (field is SetMarkerType) break;
                    if (field.IsEnumerable && !(field is ByteArrayType)) continue;
                    if (!field.HasBeenSet) break;

                }
                data.TriggeringRecordTypes.Add(recTypes);
            }

            if (isGRUP)
            {
                data.TriggeringRecordTypes.Add(new RecordType("GRUP"));
            }

            if (obj.TryGetMarkerType(out var markerType))
            {
                data.TriggeringRecordTypes.Add(markerType);
            }


            if (data.TriggeringRecordTypes.Count > 0)
            {
                if (data.TriggeringRecordTypes.CountGreaterThan(1))
                {
                    obj.GetObjectData().TriggeringSource = $"{obj.RegistrationName}.TriggeringRecordTypes";
                }
                else
                {
                    obj.GetObjectData().TriggeringSource = obj.RecordTypeHeaderName(data.TriggeringRecordTypes.First());
                }
            }
        }

        public override async Task PostLoad(ObjectGeneration obj)
        {
            await Task.WhenAll(
                obj.IterateFields(expandSets: SetMarkerType.ExpandSets.TrueAndInclude, nonIntegrated: true)
                    .Select((field) => SetRecordTrigger(obj, field, field.GetFieldData())));
            await SetObjectTrigger(obj);
            await base.PostLoad(obj);
            Dictionary<string, TypeGeneration> triggerMapping = new Dictionary<string, TypeGeneration>();
            Dictionary<RecordType, TypeGeneration> triggerRecMapping = new Dictionary<RecordType, TypeGeneration>();
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
                foreach (var triggerRec in mutaData.TriggeringRecordTypes)
                {
                    if (triggerRecMapping.TryGetValue(triggerRec, out var existingField))
                    {
                        throw new ArgumentException($"{obj.Name} cannot have two fields that have the same trigger record {triggerRec}: {existingField.Name} AND {field.Name}");
                    }
                    triggerRecMapping[triggerRec] = field;
                }
            }
        }

        private async Task AddLoquiSubTypes(LoquiType loqui)
        {
            if (loqui.TargetObjectGeneration == null || loqui.GenericDef != null) return;
            var inheritingObjs = await loqui.TargetObjectGeneration.InheritingObjects();
            var data = loqui.GetFieldData();
            foreach (var subObj in inheritingObjs)
            {
                var subRecs = await subObj.TryGetTriggeringRecordTypes();
                if (subRecs.Failed) continue;
                foreach (var subRec in subRecs.Value)
                {
                    data.SubLoquiTypes.Add(subRec, subObj);
                }
            }
        }

        private async Task SetRecordTrigger(
            ObjectGeneration obj,
            TypeGeneration field,
            MutagenFieldData data)
        {
            if (field is LoquiType loqui
                && !(field is FormIDLinkType))
            {
                IEnumerable<RecordType> trigRecTypes = await TaskExt.AwaitOrDefaultValue(loqui.TargetObjectGeneration?.TryGetTriggeringRecordTypes());
                if (loqui.TargetObjectGeneration != null
                    && (loqui.TargetObjectGeneration.TryGetRecordType(out var recType)
                        || trigRecTypes != null))
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
                        if (trigRecTypes != null)
                        {
                            data.TriggeringRecordTypes.Add(trigRecTypes);
                        }
                    }
                }
                else if (loqui.GenericDef != null)
                {
                    data.TriggeringRecordAccessors.Add($"{loqui.GenericDef.Name}_RecordType");
                }
                await AddLoquiSubTypes(loqui);
            }
            else if (field is LoquiListType loquiList
                && !data.RecordType.HasValue)
            {
                loqui = loquiList.SubTypeGeneration as LoquiType;
                var trigRecTypes = await TaskExt.AwaitOrDefault(loqui.TargetObjectGeneration?.TryGetTriggeringRecordTypes());
                if (trigRecTypes.Succeeded)
                {
                    data.TriggeringRecordTypes.Add(trigRecTypes.Value);
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
                    IEnumerable<RecordType> trigRecTypes = await TaskExt.AwaitOrDefaultValue(subListLoqui.TargetObjectGeneration?.TryGetTriggeringRecordTypes());
                    if (trigRecTypes != null)
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
                    await SetRecordTrigger(obj, listType.SubTypeGeneration, subData);
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
                await SetRecordTrigger(
                    obj,
                    contLoqui,
                    subData);
            }

            if (data.HasTrigger)
            {
                field.HasBeenSetProperty.SetIfNotSet(true);
            }
        }

        private void SetTriggeringRecordAccessors(ObjectGeneration obj, TypeGeneration field, MutagenFieldData data)
        {
            if (!data.HasTrigger)
            {
                if (data.MarkerType.HasValue)
                {
                    data.TriggeringRecordTypes.Clear();
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
                data.TriggeringRecordTypes.Add(data.RecordType.Value);
            }
            else if (data.TriggeringRecordTypes.Count == 1)
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

