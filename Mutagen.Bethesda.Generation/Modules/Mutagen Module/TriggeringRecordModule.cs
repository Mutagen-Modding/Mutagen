using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Generation
{
    public class TriggeringRecordModule : GenerationModule
    {
        public override Task PostFieldLoad(ObjectGeneration obj, TypeGeneration field, XElement node)
        {
            var data = field.CustomData.TryCreateValue(Constants.DataKey, () => new MutagenFieldData(field)) as MutagenFieldData;
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
            if (obj.IsTopLevelGroup() && (field.Name?.Equals("Items") ?? false))
            {
                DictType dict = field as DictType;
                LoquiType loqui = dict.ValueTypeGen as LoquiType;
                data.TriggeringRecordAccessors.Add($"Group<T>.T_RecordType");
            }
            return base.PostFieldLoad(obj, field, node);
        }

        public override Task PreLoad(ObjectGeneration obj)
        {
            var data = obj.GetObjectData();
            var record = obj.Node.GetAttribute("recordType");
            data.FailOnUnknown = obj.Node.GetAttribute<bool>("failOnUnknownType", defaultVal: false);
            data.CustomBinary = obj.Node.GetAttribute<bool>("customBinary", defaultVal: false);
            data.CustomBinaryEnd = obj.Node.GetAttribute<CustomEnd>("customBinaryEnd", defaultVal: CustomEnd.Off);

            var objType = obj.Node.GetAttribute(Mutagen.Bethesda.Generation.Constants.ObjectType);
            if (!Enum.TryParse<ObjectType>(objType, out var objTypeEnum))
            {
                throw new ArgumentException("Must specify object type.");
            }
            data.ObjectType = objTypeEnum;

            if (record != null)
            {
                data.RecordType = new RecordType(record);
            }
            else if (objTypeEnum == ObjectType.Group)
            {
                data.RecordType = new RecordType("GRUP");
            }

            foreach (var elem in obj.Node.Elements(XName.Get("CustomRecordTypeTrigger", LoquiGenerator.Namespace)))
            {
                obj.GetObjectData().CustomRecordTypeTriggers.Add(new RecordType(elem.Value));
            }

            if (obj.Node.TryGetAttribute("markerType", out var markerType))
            {
                var markerTypeRec = new RecordType(markerType.Value);
                data.MarkerType = markerTypeRec;
                data.RecordType = markerTypeRec;
            }
            return base.PreLoad(obj);
        }

        public override async Task LoadWrapup(ObjectGeneration obj)
        {
            try
            {
                await Task.WhenAll(
                    obj.IterateFields(expandSets: SetMarkerType.ExpandSets.TrueAndInclude, nonIntegrated: true)
                        .Select((field) => SetRecordTrigger(obj, field, field.GetFieldData())));
                await SetObjectTrigger(obj);
                obj.GetObjectData().WiringComplete.Complete();
                await base.LoadWrapup(obj);
            }
            catch (Exception ex)
            {
                obj.GetObjectData().WiringComplete.SetException(ex);
                throw;
            }
        }

        public override async Task GenerateInRegistration(ObjectGeneration obj, FileGeneration fg)
        {
            HashSet<RecordType> trigRecordTypes = new HashSet<RecordType>();
            HashSet<RecordType> recordTypes = new HashSet<RecordType>();
            if (obj.TryGetRecordType(out var recType))
            {
                recordTypes.Add(recType);
            }
            var trigRecTypes = await obj.GetObjectData().GenerationTypes;
            trigRecordTypes.Add(trigRecTypes.SelectMany((kv) => kv.Key));
            recordTypes.Add(trigRecordTypes);
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
                foreach (var subType in data.SubLoquiTypes.Keys)
                {
                    recordTypes.Add(subType);
                }
                if (field is ContainerType contType)
                {
                    if (!contType.SubTypeGeneration.TryGetFieldData(out var subData)) continue;
                    if (contType.CustomData.TryGetValue(ListBinaryTranslationGeneration.CounterRecordType, out var counterRecType)
                        && !string.IsNullOrWhiteSpace((string)counterRecType))
                    {
                        recordTypes.Add(new RecordType((string)counterRecType));
                    }
                    if (subData.HasTrigger)
                    {
                        recordTypes.Add(subData.TriggeringRecordTypes);
                    }
                }
                else if (field is DictType dict)
                {
                    switch (dict.Mode)
                    {
                        case DictMode.KeyedValue:
                            {
                                if (!dict.ValueTypeGen.TryGetFieldData(out var subData)) continue;
                                if (!subData.HasTrigger) continue;
                                recordTypes.Add(subData.TriggeringRecordTypes);
                                break;
                            }
                        case DictMode.KeyValue:
                            {
                                if (dict.KeyTypeGen.TryGetFieldData(out var subData)
                                    && subData.HasTrigger)
                                {
                                    recordTypes.Add(subData.TriggeringRecordTypes);
                                }
                                if (dict.ValueTypeGen.TryGetFieldData(out subData)
                                    && subData.HasTrigger)
                                {
                                    recordTypes.Add(subData.TriggeringRecordTypes);
                                }
                                break;
                            }
                        default:
                            throw new NotImplementedException();
                    }
                }
                else if (field is GenderedType gendered)
                {
                    if (gendered.MaleMarker.HasValue)
                    {
                        recordTypes.Add(gendered.MaleMarker.Value);
                        recordTypes.Add(gendered.FemaleMarker.Value);
                    }
                    var subData = gendered.SubTypeGeneration.GetFieldData();
                    if (subData.RecordType != null)
                    {
                        recordTypes.Add(subData.RecordType.Value);
                    }
                }
            }
            foreach (var type in recordTypes)
            {
                fg.AppendLine($"public static readonly {nameof(RecordType)} {type.Type}_HEADER = new {nameof(RecordType)}(\"{type.Type}\");");
            }
            var count = trigRecordTypes.Count();
            if (count == 1)
            {
                fg.AppendLine($"public static readonly {nameof(RecordType)} {Mutagen.Bethesda.Constants.TriggeringRecordTypeMember} = {trigRecordTypes.First().Type}_HEADER;");
            }
            else if (count > 1)
            {
                fg.AppendLine($"public static ICollectionGetter<RecordType> TriggeringRecordTypes => _TriggeringRecordTypes.Value;");
                fg.AppendLine($"private static readonly Lazy<ICollectionGetter<RecordType>> _TriggeringRecordTypes = new Lazy<ICollectionGetter<RecordType>>(() =>");
                using (new BraceWrapper(fg) { AppendSemicolon = true, AppendParenthesis = true })
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
                                    foreach (var trigger in trigRecordTypes)
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
            await base.GenerateInRegistration(obj, fg);
        }

        private async Task SetContainerSubTriggers(
            ObjectGeneration obj,
            TypeGeneration field)
        {
            if (field is ContainerType contType
                && contType.SubTypeGeneration is LoquiType contLoqui)
            {
                var subData = contLoqui.CustomData.TryCreateValue(Constants.DataKey, () => new MutagenFieldData(contLoqui)) as MutagenFieldData;
                if (contType.CustomData.TryGetValue(ListBinaryTranslationGeneration.CounterRecordType, out var counterTypeObj)
                    && counterTypeObj is string counterType)
                {
                    subData.TriggeringRecordTypes.Add(new RecordType(counterType));
                }
                await SetRecordTrigger(
                    obj,
                    contLoqui,
                    subData);
            }
            else if (field is DictType dictType)
            {
                switch (dictType.Mode)
                {
                    case DictMode.KeyedValue:
                        if (dictType.ValueTypeGen is LoquiType dictLoqui)
                        {
                            var subData = dictLoqui.CustomData.TryCreateValue(Constants.DataKey, () => new MutagenFieldData(dictLoqui)) as MutagenFieldData;
                            await SetRecordTrigger(
                                obj,
                                dictLoqui,
                                subData);
                        }
                        break;
                    case DictMode.KeyValue:
                        if (dictType.KeyTypeGen is LoquiType || dictType.ValueTypeGen is LoquiType)
                        {
                            throw new NotImplementedException();
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private async Task SetRecordTrigger(
            ObjectGeneration obj,
            TypeGeneration field,
            MutagenFieldData data)
        {
            await SetContainerSubTriggers(obj, field);

            if (field is LoquiType loqui
                && !(field is FormLinkType))
            {
                IEnumerable<RecordType> trigRecTypes = await TaskExt.AwaitOrDefaultValue(loqui.TargetObjectGeneration?.TryGetTriggeringRecordTypes());
                if (loqui.TargetObjectGeneration != null
                    && (loqui.TargetObjectGeneration.TryGetRecordType(out var recType)
                        || trigRecTypes != null))
                {
                    var targetObjectData = loqui.TargetObjectGeneration.GetObjectData();
                    if (targetObjectData.ObjectType == ObjectType.Group
                        && loqui.GenericSpecification.Specifications.TryGetValue("T", out var objName))
                    {
                        var nameKey = ObjectNamedKey.Factory(objName);
                        var grupObj = obj.ProtoGen.Gen.ObjectGenerationsByObjectNameKey[nameKey];
                        if (grupObj.GetObjectType() == ObjectType.Group)
                        {
                            data.RecordType = (await grupObj.GetGroupLoquiTypeLowest()).TargetObjectGeneration.GetRecordType();
                        }
                        else
                        {
                            data.RecordType = grupObj.GetRecordType();
                        }
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
                            foreach (var t in trigRecTypes)
                            {
                                data.TriggeringRecordTypes.Add(data.RecordTypeConverter.ConvertToCustom(t));
                            }
                        }
                    }
                    await AddLoquiSubTypes(loqui);
                }
                else if (loqui.GenericDef != null)
                {
                    data.TriggeringRecordAccessors.Add($"{loqui.ObjectGen.ObjectName}.{loqui.GenericDef.Name}_RecordType");
                    await AddLoquiSubTypes(loqui);
                }
                else if (loqui.RefType == LoquiType.LoquiRefType.Interface)
                {
                    var implementingObjs = obj.ProtoGen.ObjectGenerationsByID.Values
                        .Where(o => o.Interfaces.ContainsAtLeast(loqui.GetterInterface, LoquiInterfaceType.Direct)
                            || o.Interfaces.ContainsAtLeast(loqui.SetterInterface, LoquiInterfaceType.Direct))
                        .ToArray();
                    await loqui.AddAsSubLoquiType(implementingObjs);
                }
            }
            else if (field is ListType listType
                && !data.RecordType.HasValue)
            {
                if (listType.SubTypeGeneration is LoquiType subListLoqui)
                {
                    if (subListLoqui.GenericDef != null)
                    {
                        data.TriggeringRecordAccessors.Add($"{subListLoqui.ObjectGen.ObjectName}.{subListLoqui.GenericDef.Name}_RecordType");
                    }
                    else
                    {
                        var subData = subListLoqui.GetFieldData();
                        foreach (var gen in subData.GenerationTypes)
                        {
                            data.TriggeringRecordTypes.Add(gen.Key);
                        }
                    }
                }
                else
                {
                    var subData = listType.SubTypeGeneration.CustomData.TryCreateValue(Constants.DataKey, () => new MutagenFieldData(listType.SubTypeGeneration)) as MutagenFieldData;
                    await SetRecordTrigger(obj, listType.SubTypeGeneration, subData);
                    if (subData.HasTrigger)
                    {
                        data.TriggeringRecordAccessors.Add(obj.RecordTypeHeaderName(subData.RecordType.Value));
                        data.TriggeringRecordTypes.Add(subData.RecordType.Value);
                        data.RecordType = subData.RecordType;
                        // Don't actually want it to be marked has been set
                        listType.SubTypeGeneration.HasBeenSetProperty.OnNext((false, true));
                    }
                }
            }
            else if (field is DictType dictType
                && !data.RecordType.HasValue)
            {
                if (dictType.ValueTypeGen is LoquiType subDictLoqui)
                {
                    if (subDictLoqui.GenericDef != null)
                    {
                        data.TriggeringRecordAccessors.Add($"{subDictLoqui.ObjectGen.ObjectName}.{subDictLoqui.GenericDef.Name}_RecordType");
                    }
                    else
                    {
                        var subData = subDictLoqui.GetFieldData();
                        foreach (var gen in subData.GenerationTypes)
                        {
                            data.TriggeringRecordTypes.Add(gen.Key);
                        }
                    }
                }
                else
                {
                    var subData = dictType.ValueTypeGen.CustomData.TryCreateValue(Constants.DataKey, () => new MutagenFieldData(dictType.ValueTypeGen)) as MutagenFieldData;
                    await SetRecordTrigger(obj, dictType.ValueTypeGen, subData);
                    if (subData.HasTrigger)
                    {
                        data.TriggeringRecordAccessors.Add(obj.RecordTypeHeaderName(subData.RecordType.Value));
                        data.TriggeringRecordTypes.Add(subData.RecordType.Value);
                        data.RecordType = subData.RecordType;
                    }
                }
            }
            else if (field is GenderedType gendered)
            {
                if (!data.MarkerType.HasValue
                    && gendered.ItemHasBeenSet)
                {
                    data.TriggeringRecordAccessors.Add(obj.RecordTypeHeaderName(gendered.MaleMarker.Value));
                    data.TriggeringRecordAccessors.Add(obj.RecordTypeHeaderName(gendered.FemaleMarker.Value));
                    data.TriggeringRecordTypes.Add(gendered.MaleMarker.Value);
                    data.TriggeringRecordTypes.Add(gendered.FemaleMarker.Value);
                }
            }

            SetTriggeringRecordAccessors(obj, field, data);

            if (!field.HasBeenSetProperty.Value.HasBeenSet)
            {
                field.HasBeenSetProperty.OnNext((data.HasTrigger, true));
            }
        }
        
        private async Task AddLoquiSubTypes(LoquiType loqui)
        {
            if (loqui.TargetObjectGeneration == null || loqui.GenericDef != null) return;
            var inheritingObjs = await loqui.TargetObjectGeneration.InheritingObjects();
            await loqui.AddAsSubLoquiType(inheritingObjs);
        }

        private async Task SetBasicTriggers(
            ObjectGeneration obj,
            MutagenObjData data,
            bool isGRUP)
        {
            if (obj.TryGetRecordType(out var recType) && !isGRUP)
            {
                data.TriggeringRecordTypes.Add(recType);
                return;
            }

            if (obj.HasLoquiBaseObject)
            {
                var baseTrigger = await obj.BaseClass.TryGetTriggeringRecordTypes();
                if (baseTrigger.Succeeded)
                {
                    if (data.BaseRecordTypeConverter != null)
                    {
                        data.TriggeringRecordTypes.Add(
                            baseTrigger.Value.Select((b) =>
                            {
                                return data.BaseRecordTypeConverter.ConvertToCustom(b);
                            }));
                    }
                    else
                    {
                        data.TriggeringRecordTypes.Add(baseTrigger.Value);
                    }
                    return;
                }
            }

            HashSet<RecordType> recTypes = new HashSet<RecordType>();
            foreach (var field in obj.IterateFields(
                nonIntegrated: true,
                expandSets: SetMarkerType.ExpandSets.FalseAndInclude))
            {
                if (!field.IntegrateField
                    && !(field is DataType)
                    && !(field is SpecialParseType)
                    && !(field is CustomLogic)) continue;
                if (!field.TryGetFieldData(out var fieldData)) break;
                if (!fieldData.HasTrigger) break;
                recTypes.Add(fieldData.TriggeringRecordTypes);
                fieldData.IsTriggerForObject = true;
                if (field is SetMarkerType) break;
                if (field.IsEnumerable && !(field is ByteArrayType)) continue;
                LoquiType loqui = field as LoquiType;
                if (!field.HasBeenSet && (loqui == null || loqui.SingletonType == SingletonLevel.None)) break;
            }
            data.TriggeringRecordTypes.Add(recTypes);
        }

        private async Task SetObjectTrigger(ObjectGeneration obj)
        {
            var data = obj.GetObjectData();
            await SetBasicTriggers(obj, data, isGRUP: data.ObjectType == ObjectType.Group);

            if (data.ObjectType == ObjectType.Group)
            {
                data.TriggeringRecordTypes.Add(new RecordType("GRUP"));
            }

            if (obj.TryGetMarkerType(out var markerType))
            {
                data.TriggeringRecordTypes.Add(markerType);
            }

            if (obj.TryGetCustomRecordTypeTriggers(out var customTypeTriggers))
            {
                data.TriggeringRecordTypes.Add(customTypeTriggers);
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
            if (field is ContainerType cont)
            {
                if (field.CustomData.TryGetValue(ListBinaryTranslationGeneration.CounterRecordType, out var counterObj)
                    && counterObj is string counterTypeStr)
                {
                    data.TriggeringRecordTypes.Clear();
                    data.TriggeringRecordAccessors.Add(obj.RecordTypeHeaderName(new RecordType(counterTypeStr)));
                    data.TriggeringRecordTypes.Add(new RecordType(counterTypeStr));
                }
            }
            if (data.RecordType.HasValue
                && data.TriggeringRecordTypes.Count < 1)
            {
                data.TriggeringRecordSetAccessor = obj.RecordTypeHeaderName(data.RecordType.Value);
                data.TriggeringRecordTypes.Add(data.RecordType.Value);
            }
            else if (data.TriggeringRecordTypes.Count == 1
                && data.SubLoquiTypes.Count == 0)
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

        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            if (obj.GetObjectType() == ObjectType.Group)
            {
                var grupLoqui = await obj.GetGroupLoquiType();
                if (grupLoqui.GenericDef == null)
                {
                    fg.AppendLine($"public static readonly {nameof(RecordType)} {Mutagen.Bethesda.Constants.GrupRecordTypeMember} = (RecordType){grupLoqui.TargetObjectGeneration.Name}.{Mutagen.Bethesda.Constants.GrupRecordTypeMember};");
                }
            }
            else if (await obj.IsSingleTriggerSource())
            {
                await obj.IsSingleTriggerSource();
                fg.AppendLine($"public new static readonly {nameof(RecordType)} {Mutagen.Bethesda.Constants.GrupRecordTypeMember} = {obj.RegistrationName}.{Mutagen.Bethesda.Constants.TriggeringRecordTypeMember};");
            }
            await base.GenerateInClass(obj, fg);
        }
    }
}
