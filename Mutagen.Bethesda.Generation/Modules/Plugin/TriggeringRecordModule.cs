using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Generation.Modules.Binary;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;
using System.Xml.Linq;
using Mutagen.Bethesda.Generation.Fields;
using DictType = Mutagen.Bethesda.Generation.Fields.DictType;
using BoolType = Mutagen.Bethesda.Generation.Fields.BoolType;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class TriggeringRecordModule : GenerationModule
{
    public override Task PostFieldLoad(ObjectGeneration obj, TypeGeneration field, XElement node)
    {
        var data = field.CustomData.GetOrAdd(Constants.DataKey, () => new MutagenFieldData(field)) as MutagenFieldData;
        if (node.TryGetAttribute("recordType", out string recordAttr))
        {
            data.RecordType = new RecordType(recordAttr);
        }
        else if (node.TryGetAttribute("recordTypeHex", out string recordAttrInt))
        {
            data.RecordType = new RecordType(Convert.ToInt32(recordAttrInt, 16));
        }
        if (node.TryGetAttribute("overflowRecordType", out var overflow))
        {
            data.OverflowRecordType = new RecordType(overflow.Value);
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
        data.UsesStringFiles = obj.Node.GetAttribute<bool>("usesStringFiles", defaultVal: true);
        data.CustomBinaryEnd = obj.Node.GetAttribute<CustomEnd>("customBinaryEnd", defaultVal: CustomEnd.Off);
        data.BinaryOverlay = obj.Node.GetAttribute<BinaryGenerationType>("binaryOverlay", defaultVal: BinaryGenerationType.Normal);

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
            data.RecordType = Plugins.Internals.Constants.Group;
        }

        foreach (var elem in obj.Node.Elements(XName.Get("CustomRecordTypeTrigger", LoquiGenerator.Namespace)))
        {
            obj.GetObjectData().CustomRecordTypeTriggers.Add(new RecordType(elem.Value));
        }

        foreach (var elem in obj.Node.Elements(XName.Get("AdditionalContainedRecordType", LoquiGenerator.Namespace)))
        {
            obj.GetObjectData().AdditionalContainedRecordTypes.Add(new RecordType(elem.Value));
        }

        if (obj.Node.TryGetAttribute("markerType", out var markerType))
        {
            var markerTypeRec = new RecordType(markerType.Value);
            data.MarkerType = markerTypeRec;
        }
        if (obj.Node.TryGetAttribute("endMarkerType", out var endMarker))
        {
            var markerTypeRec = new RecordType(endMarker.Value);
            data.EndMarkerType = markerTypeRec;
        }
        return base.PreLoad(obj);
    }

    public override async Task LoadWrapup(ObjectGeneration obj)
    {
        try
        {
            await Task.WhenAll(
                obj.IterateFields(expandSets: SetMarkerType.ExpandSets.TrueAndInclude, nonIntegrated: true)
                    .Select(async (field) =>
                    {
                        await SetContainerSubTriggers(obj, field);
                    }));
            foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.TrueAndInclude, nonIntegrated: true))
            {
                await SetRecordTrigger(obj, field, field.GetFieldData());
            }
            await SetObjectTrigger(obj);
            obj.GetObjectData().WiringComplete.SetResult();
            await base.LoadWrapup(obj);
        }
        catch (Exception ex)
        {
            obj.GetObjectData().WiringComplete.SetException(ex);
            throw;
        }
    }

    public override async IAsyncEnumerable<string> RequiredUsingStatements(ObjectGeneration obj)
    {
        yield return $"{obj.ProtoGen.DefaultNamespace}.Internals";
        yield return $"Mutagen.Bethesda.Plugins.Internals";
    }

    public async IAsyncEnumerable<RecordType> GetAllRecordTypes(ObjectGeneration obj)
    {
        HashSet<RecordType> recordTypes = new HashSet<RecordType>();
        if (obj.TryGetRecordType(out var recType))
        {
            recordTypes.Add(recType);
        }
        var data = obj.GetObjectData();
        var trigRecTypes = await data.GenerationTypes;
        recordTypes.Add(trigRecTypes.SelectMany((kv) => kv.Key));
        if (data.EndMarkerType.HasValue)
        {
            recordTypes.Add(data.EndMarkerType.Value);
        }
        recordTypes.Add(data.AdditionalContainedRecordTypes);
        foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.FalseAndInclude, nonIntegrated: true))
        {
            var fieldData = field.GetFieldData();
            if (fieldData.RecordTypeConverter != null)
            {
                foreach (var type in fieldData.RecordTypeConverter.FromConversions.Values)
                {
                    recordTypes.Add(type);
                }
            }
            if (fieldData.RecordType.HasValue)
            {
                recordTypes.Add(fieldData.RecordType.Value);
            }
            recordTypes.Add(fieldData.TriggeringRecordTypes);
            if (fieldData.MarkerType.HasValue)
            {
                recordTypes.Add(fieldData.MarkerType.Value);
            }
            if (fieldData.OverflowRecordType.HasValue)
            {
                recordTypes.Add(fieldData.OverflowRecordType.Value);
            }
            foreach (var subType in fieldData.SubLoquiTypes.Keys)
            {
                recordTypes.Add(subType);
            }
            if (field is ContainerType contType)
            {
                var subData = contType.SubTypeGeneration.GetFieldData();
                if (contType.CustomData.TryGetValue(PluginListBinaryTranslationGeneration.CounterRecordType, out var counterRecType)
                    && !string.IsNullOrWhiteSpace((string)counterRecType))
                {
                    recordTypes.Add(new RecordType((string)counterRecType));
                }
                if (subData.HasTrigger)
                {
                    recordTypes.Add(subData.TriggeringRecordTypes);
                }
                if (contType.SubTypeGeneration is LoquiType loqui
                    && loqui.TargetObjectGeneration != null)
                {
                    recordTypes.Add(await GetAllRecordTypes(loqui.TargetObjectGeneration).ToArrayAsync());
                }
                if (contType.CustomData.TryGetValue(PluginListBinaryTranslationGeneration.EndMarker, out var endMarkerObj) && endMarkerObj is string endMarker)
                {
                    recordTypes.Add(endMarker);
                }
            }
            else if (field is DictType dict)
            {
                switch (dict.Mode)
                {
                    case DictMode.KeyedValue:
                        {
                            var subData = dict.ValueTypeGen.GetFieldData();
                            if (!subData.HasTrigger) continue;
                            recordTypes.Add(subData.TriggeringRecordTypes);
                            break;
                        }
                    case DictMode.KeyValue:
                        {
                            var subData = dict.KeyTypeGen.GetFieldData();
                            if (subData.HasTrigger)
                            {
                                recordTypes.Add(subData.TriggeringRecordTypes);
                            }
                            subData = dict.ValueTypeGen.GetFieldData();
                            if (subData.HasTrigger)
                            {
                                recordTypes.Add(subData.TriggeringRecordTypes);
                            }
                            break;
                        }
                    default:
                        throw new NotImplementedException();
                }
                if (dict.ValueTypeGen is LoquiType loqui
                    && loqui.TargetObjectGeneration != null)
                {
                    recordTypes.Add(await GetAllRecordTypes(loqui.TargetObjectGeneration).ToArrayAsync());
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
                if (gendered.MaleConversions != null)
                {
                    foreach (var type in gendered.MaleConversions.FromConversions.Values)
                    {
                        recordTypes.Add(type);
                    }
                }
                if (gendered.FemaleConversions != null)
                {
                    foreach (var type in gendered.FemaleConversions.FromConversions.Values)
                    {
                        recordTypes.Add(type);
                    }
                }
            }
        }
        foreach (var item in recordTypes)
        {
            yield return item;
        }
    }

    public override async Task GenerateInRegistration(ObjectGeneration obj, FileGeneration fg)
    {
        HashSet<RecordType> trigRecordTypes = new HashSet<RecordType>();
        var data = obj.GetObjectData();
        trigRecordTypes.Add((await data.GenerationTypes).SelectMany((kv) => kv.Key));
        var count = trigRecordTypes.Count();
        if (obj.Name.EndsWith("MajorRecord")) return;
        if (count == 1)
        {
            fg.AppendLine($"public static readonly {nameof(RecordType)} {Plugins.Internals.Constants.TriggeringRecordTypeMember} = {obj.RecordTypeHeaderName(trigRecordTypes.First())};");
        }
        if (count >= 1)
        {
            fg.AppendLine($"public static ITriggeringRecordCollection TriggeringRecordTypes => _TriggeringRecordTypes.Value;");
            fg.AppendLine($"private static readonly Lazy<ITriggeringRecordCollection> _TriggeringRecordTypes = new Lazy<ITriggeringRecordCollection>(() =>");
            using (new BraceWrapper(fg) { AppendSemicolon = true, AppendParenthesis = true })
            {
                using (var args = new ArgsWrapper(fg,
                    "return TriggeringRecordCollection.Factory"))
                {
                    foreach (var trigger in trigRecordTypes)
                    {
                        args.Add($"{obj.RecordTypeHeaderName(trigger)}");
                    }
                }
            }
        }

        if (obj.GetObjectType() != ObjectType.Group && obj.GetObjectType() != ObjectType.Mod)
        {
            var all = await GetAllRecordTypes(obj).ToArrayAsync();
            if (count > 1 && trigRecordTypes.ToHashSet().Equals(all.ToHashSet()))
            {
                fg.AppendLine($"public static ITriggeringRecordCollection AllRecordTypes => _TriggeringRecordTypes.Value;");
            }
            else if (all.Length == 1)
            {
                fg.AppendLine($"public static RecordType AllRecordTypes => {Plugins.Internals.Constants.TriggeringRecordTypeMember};");
            }
            else if (count > 0)
            {
                fg.AppendLine($"public static ITriggeringRecordCollection AllRecordTypes => _AllRecordTypes.Value;");
                fg.AppendLine($"private static readonly Lazy<ITriggeringRecordCollection> _AllRecordTypes = new Lazy<ITriggeringRecordCollection>(() =>");
                using (new BraceWrapper(fg) { AppendSemicolon = true, AppendParenthesis = true })
                {
                    using (var args = new ArgsWrapper(fg,
                        "return TriggeringRecordCollection.Factory"))
                    {
                        foreach (var trigger in all)
                        {
                            args.Add($"{obj.RecordTypeHeaderName(trigger)}");
                        }
                    }
                }
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
            var nullable = contType.SubTypeGeneration.Nullable;
            var subData = contLoqui.CustomData.GetOrAdd(Constants.DataKey, () => new MutagenFieldData(contLoqui)) as MutagenFieldData;
            await SetRecordTrigger(
                obj,
                contLoqui,
                subData);
            if (contType.CustomData.TryGetValue(PluginListBinaryTranslationGeneration.CounterRecordType, out var counterTypeObj)
                && counterTypeObj is string counterType
                && !subData.HasTrigger)
            {
                subData.TriggeringRecordTypes.Add(new RecordType(counterType));
            }
            contType.SubTypeGeneration.NullableProperty.OnNext((nullable, true));
        }
        else if (field is DictType dictType)
        {
            switch (dictType.Mode)
            {
                case DictMode.KeyedValue:
                    if (dictType.ValueTypeGen is LoquiType dictLoqui)
                    {
                        var subData = dictLoqui.CustomData.GetOrAdd(Constants.DataKey, () => new MutagenFieldData(dictLoqui)) as MutagenFieldData;
                        await SetRecordTrigger(
                            obj,
                            dictLoqui,
                            subData);
                    }
                    break;
                case DictMode.KeyValue:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        else if (field is GenderedType gendered)
        {
            if (gendered.SubTypeGeneration is LoquiType loqui)
            {
                await SetRecordTrigger(
                    obj,
                    loqui,
                    loqui.GetFieldData());
            }
        }
    }

    private async Task SetRecordTrigger(
        ObjectGeneration obj,
        TypeGeneration field,
        MutagenFieldData data)
    {
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
                    .Where(o => o.Interfaces.ContainsAtLeast(loqui.GetterInterface, LoquiInterfaceDefinitionType.Direct)
                        || o.Interfaces.ContainsAtLeast(loqui.SetterInterface, LoquiInterfaceDefinitionType.Direct))
                    .ToArray();
                await loqui.AddAsSubLoquiType(implementingObjs);
            }
        }
        else if (field is ListType listType
            && !data.RecordType.HasValue)
        {
            bool previouslyTurnedOff = false;
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
                var subData = listType.SubTypeGeneration.CustomData.GetOrAdd(Constants.DataKey, () => new MutagenFieldData(listType.SubTypeGeneration)) as MutagenFieldData;
                await SetRecordTrigger(obj, listType.SubTypeGeneration, subData);
                if (subData.HasTrigger)
                {
                    data.TriggeringRecordAccessors.Add(obj.RecordTypeHeaderName(subData.RecordType.Value));
                    data.TriggeringRecordTypes.Add(subData.RecordType.Value);
                    data.RecordType = subData.RecordType;
                    // Don't actually want it to be marked has been set
                    listType.SubTypeGeneration.NullableProperty.OnNext((false, true));
                    listType.NullableProperty.OnNext((false, true));
                    previouslyTurnedOff = true;
                }
            }
            // If has count, mark as has been set
            var hasCounter = (listType.CustomData.TryGetValue(PluginListBinaryTranslationGeneration.CounterRecordType, out var counter)
                              && counter != null);
            if (hasCounter 
                && (previouslyTurnedOff || !listType.NullableProperty.Value.HasBeenSet))
            {
                listType.NullableProperty.OnNext((true, true));
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
                var subData = dictType.ValueTypeGen.CustomData.GetOrAdd(Constants.DataKey, () => new MutagenFieldData(dictType.ValueTypeGen)) as MutagenFieldData;
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
                && gendered.MaleMarker.HasValue
                && gendered.ItemNullable)
            {
                data.TriggeringRecordAccessors.Add(obj.RecordTypeHeaderName(gendered.MaleMarker.Value));
                data.TriggeringRecordAccessors.Add(obj.RecordTypeHeaderName(gendered.FemaleMarker.Value));
                data.TriggeringRecordTypes.Add(gendered.MaleMarker.Value);
                data.TriggeringRecordTypes.Add(gendered.FemaleMarker.Value);
            }
            else if (gendered.SubTypeGeneration is LoquiType genderedLoqui)
            {
                foreach (var gen in genderedLoqui.GetFieldData().GenerationTypes)
                {
                    foreach (var recordType in gen.Key)
                    {
                        bool subbedFemale = false, subbedMale = false;
                        if (gendered.MaleConversions != null 
                            && gendered.MaleConversions.FromConversions.TryGetValue(recordType, out var maleSub))
                        {
                            subbedMale = true;
                            data.TriggeringRecordTypes.Add(maleSub);
                        }
                        if (gendered.FemaleConversions != null
                            && gendered.FemaleConversions.FromConversions.TryGetValue(recordType, out var femaleSub))
                        {
                            subbedFemale = true;
                            data.TriggeringRecordTypes.Add(femaleSub);
                        }
                        if (!subbedFemale || !subbedMale)
                        {
                            data.TriggeringRecordTypes.Add(recordType);
                        }
                    }
                }
            }
        }

        SetTriggeringRecordAccessors(obj, field, data);

        if (!field.NullableProperty.Value.HasBeenSet)
        {
            bool nullable;
            switch (field)
            {
                case ListType list:
                    nullable = data.RecordType != null 
                        || (list.CustomData.TryGetValue(PluginListBinaryTranslationGeneration.CounterRecordType, out var counter) && counter != null);
                    break;
                default:
                    nullable = data.HasTrigger;
                    break;
            }
            field.NullableProperty.OnNext((nullable, true));
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
                && !(field is MarkerType)
                && !(field is CustomLogic)) continue;
            var fieldData = field.GetFieldData();
            if (!fieldData.HasTrigger) break;
            recTypes.Add(fieldData.TriggeringRecordTypes);
            fieldData.IsTriggerForObject = true;
            if (field is SetMarkerType) break;
            if (field.IsEnumerable && !(field is ByteArrayType)) continue;
            LoquiType loqui = field as LoquiType;
            if (!field.Nullable 
                && fieldData.Binary != BinaryGenerationType.Custom
                && (field is not BoolType bt || !bt.BoolAsMarker.HasValue)
                && !(field is CustomLogic))
            {
                break;
            }
        }
        data.TriggeringRecordTypes.Add(recTypes);

        if (data.GameReleaseConverters != null)
        {
            foreach (var trigger in data.TriggeringRecordTypes.ToList())
            {
                foreach (var gameConv in data.GameReleaseConverters)
                {
                    data.TriggeringRecordTypes.Add(gameConv.Value.ConvertToCustom(trigger));
                }
            }
        }

        if (data.VersionConverters != null)
        {
            foreach (var trigger in data.TriggeringRecordTypes.ToList())
            {
                foreach (var gameConv in data.VersionConverters)
                {
                    data.TriggeringRecordTypes.Add(gameConv.Value.ConvertToCustom(trigger));
                }
            }
        }
    }

    private async Task SetObjectTrigger(ObjectGeneration obj)
    {
        var data = obj.GetObjectData();
        await SetBasicTriggers(obj, data, isGRUP: data.ObjectType == ObjectType.Group);

        if (data.ObjectType == ObjectType.Group)
        {
            data.TriggeringRecordTypes.Add(Plugins.Internals.Constants.Group);
        }

        if (obj.TryGetMarkerType(out var markerType))
        {
            data.TriggeringRecordTypes.Add(markerType);
        }

        if (obj.TryGetCustomRecordTypeTriggers(out var customTypeTriggers))
        {
            data.TriggeringRecordTypes.Add(customTypeTriggers);
        }

        if (data.GameReleaseConverters != null)
        {
            foreach (var trigger in data.TriggeringRecordTypes.ToList())
            {
                foreach (var gameConv in data.GameReleaseConverters)
                {
                    data.TriggeringRecordTypes.Add(gameConv.Value.ConvertToCustom(trigger));
                }
            }
        }

        if (data.VersionConverters != null)
        {
            foreach (var trigger in data.TriggeringRecordTypes.ToList())
            {
                foreach (var gameConv in data.VersionConverters)
                {
                    data.TriggeringRecordTypes.Add(gameConv.Value.ConvertToCustom(trigger));
                }
            }
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
        var loqui = field as LoquiType;
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
            if (field.CustomData.TryGetValue(PluginListBinaryTranslationGeneration.CounterRecordType, out var counterObj)
                && counterObj is string counterTypeStr)
            {
                var allowNoCounter = (bool)field.CustomData[PluginListBinaryTranslationGeneration.AllowNoCounter];
                if (!allowNoCounter)
                {
                    data.TriggeringRecordTypes.Clear();
                }
                data.TriggeringRecordAccessors.Add(obj.RecordTypeHeaderName(new RecordType(counterTypeStr)));
                data.TriggeringRecordTypes.Add(new RecordType(counterTypeStr));
            }
        }
        if (data.RecordType.HasValue
            && data.TriggeringRecordTypes.Count == 0)
        {
            data.TriggeringRecordSetAccessor = obj.RecordTypeHeaderName(data.RecordType.Value);
            data.TriggeringRecordTypes.Add(data.RecordType.Value);
        }
        else if (data.TriggeringRecordTypes.Count == 1
            && data.SubLoquiTypes.Count == 0)
        {
            data.TriggeringRecordSetAccessor = obj.RecordTypeHeaderName(data.TriggeringRecordTypes.First());
        }
        else if (loqui != null)
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
        if (loqui?.TargetObjectGeneration != null)
        {
            data.AllRecordSetAccessor = $"{loqui.TargetObjectGeneration.RegistrationName}.AllRecordTypes";
        }
    }

    public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
    {
        if (obj.GetObjectType() == ObjectType.Group)
        {
            var grupLoqui = await obj.GetGroupLoquiType();
            if (grupLoqui.GenericDef == null)
            {
                fg.AppendLine($"public static readonly {nameof(RecordType)} {Plugins.Internals.Constants.GrupRecordTypeMember} = (RecordType){grupLoqui.TargetObjectGeneration.Name}.{Plugins.Internals.Constants.GrupRecordTypeMember};");
            }
        }
        else if (await obj.IsSingleTriggerSource())
        {
            await obj.IsSingleTriggerSource();
            fg.AppendLine($"public{obj.NewOverride(b => !b.Abstract)}static readonly {nameof(RecordType)} {Plugins.Internals.Constants.GrupRecordTypeMember} = {obj.RegistrationName}.{Plugins.Internals.Constants.TriggeringRecordTypeMember};");
        }
        await base.GenerateInClass(obj, fg);
    }

    public override async Task FinalizeGeneration(ProtocolGeneration proto)
    {
        await base.FinalizeGeneration(proto);
        HashSet<RecordType> recordTypes = new HashSet<RecordType>();
        foreach (var obj in proto.ObjectGenerationsByID.Values)
        {
            recordTypes.Add(GetAllRecordTypes(obj).ToEnumerable());
        }
        FileGeneration fg = new FileGeneration();
        fg.AppendLine("using Mutagen.Bethesda.Plugins;");
        fg.AppendLine();

        using (var n = new NamespaceWrapper(fg, $"{proto.DefaultNamespace}.Internals"))
        {
            using (var c = new ClassWrapper(fg, "RecordTypes"))
            {
                c.Partial = true;
            }
            using (new BraceWrapper(fg))
            {
                foreach (var type in recordTypes.OrderBy(r => r.Type))
                {
                    fg.AppendLine($"public static readonly {nameof(RecordType)} {type.CheckedType} = new(0x{type.TypeInt:X});");
                }
            }
        }
        var path = Path.Combine(proto.DefFileLocation.FullName, $"RecordTypes{Loqui.Generation.Constants.AutogeneratedMarkerString}.cs");
        fg.Generate(path);
        proto.GeneratedFiles.Add(path, ProjItemType.Compile);
        fg = new FileGeneration();
        using (var n = new NamespaceWrapper(fg, $"{proto.DefaultNamespace}.Internals"))
        {
            using (var c = new ClassWrapper(fg, "RecordTypeInts"))
            {
                c.Partial = true;
            }
            using (new BraceWrapper(fg))
            {
                foreach (var type in recordTypes.OrderBy(r => r.Type))
                {
                    fg.AppendLine($"public const int {type.CheckedType} = 0x{type.TypeInt:X};");
                }
            }
        }
        path = Path.Combine(proto.DefFileLocation.FullName, $"RecordTypeInts{Loqui.Generation.Constants.AutogeneratedMarkerString}.cs");
        fg.Generate(path);
        proto.GeneratedFiles.Add(path, ProjItemType.Compile);
    }
}
