using Loqui.Generation;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using Mutagen.Bethesda.Generation.Fields;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using ObjectType = Mutagen.Bethesda.Plugins.Meta.ObjectType;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class LoquiBinaryTranslationGeneration : BinaryTranslationGeneration
{
    public const string AsyncOverrideKey = "AsyncOverride";

    public string TranslationTerm;
    public override bool DoErrorMasks => false;
    public override bool IsAsync(TypeGeneration gen, bool read)
    {
        if (!read) return false;
        LoquiType loqui = gen as LoquiType;
        if (loqui.CustomData.TryGetValue(AsyncOverrideKey, out var asyncOverride))
        {
            return (bool)asyncOverride;
        }
        if (loqui.TargetObjectGeneration != null)
        {
            if (loqui.TargetObjectGeneration.GetObjectData().CustomBinaryEnd == CustomEnd.Async) return true;
            return this.Module.HasAsync(loqui.TargetObjectGeneration, self: true);
        }
        return false;
    }

    public override bool AllowDirectWrite(ObjectGeneration objGen, TypeGeneration typeGen)
    {
        return false;
    }

    public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
    {
        var loquiGen = typeGen as LoquiType;
        if (loquiGen.CanStronglyType)
        {
            return $"LoquiBinaryTranslation<{loquiGen.TypeName(getter: getter)}>.Instance";
        }
        else
        {
            return $"LoquiBinaryTranslation.Instance";
        }
    }

    public LoquiBinaryTranslationGeneration(string translationTerm)
    {
        this.TranslationTerm = translationTerm;
    }

    public override async Task GenerateWrite(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor writerAccessor,
        Accessor itemAccessor,
        Accessor errorMaskAccessor,
        Accessor translationMaskAccessor,
        Accessor converterAccessor)
    {
        var loquiGen = typeGen as LoquiType;
        bool isGroup = objGen.GetObjectType() == ObjectType.Mod
                       && loquiGen.TargetObjectGeneration.GetObjectData().ObjectType == ObjectType.Group;
        if (typeGen.Nullable)
        {
            sb.AppendLine($"if ({itemAccessor} is {{}} {typeGen.Name}Item)");
            itemAccessor = $"{typeGen.Name}Item";
        }
        else
        {
            // We want to cache retrievals, in case it's a wrapper being written 
            sb.AppendLine($"var {typeGen.Name}Item = {itemAccessor};");
            itemAccessor = $"{typeGen.Name}Item";
        }
        using (sb.CurlyBrace(doIt: typeGen.Nullable))
        {
            if (isGroup)
            {
                var dictGroup = loquiGen.TargetObjectGeneration.Name == $"{objGen.ProtoGen.Protocol.Namespace}Group";
                sb.AppendLine($"if ({itemAccessor}.{(dictGroup ? "RecordCache" : "Records")}.Count > 0)");
            }
            using (sb.CurlyBrace(doIt: isGroup))
            {
                var data = loquiGen.GetFieldData();
                    
                if (data.MarkerType.HasValue)
                {
                    sb.AppendLine($"using ({nameof(HeaderExport)}.{nameof(HeaderExport.Subrecord)}(writer, {objGen.RecordTypeHeaderName(data.MarkerType.Value)})) {{ }}");
                }
                var needsHeaderWrite = false;
                if (NeedsHeaderProcessing(loquiGen))
                {
                    needsHeaderWrite = true;
                    sb.AppendLine($"using ({nameof(HeaderExport)}.{nameof(HeaderExport.Subrecord)}(writer, {loquiGen.GetFieldData().TriggeringRecordAccessor}))");
                }
                using (sb.CurlyBrace(doIt: needsHeaderWrite))
                {
                    string line;
                    if (loquiGen.TargetObjectGeneration != null)
                    {
                        line = $"(({this.Module.TranslationWriteClassName(loquiGen.TargetObjectGeneration)})(({Module.TranslationItemInterface}){itemAccessor}).{this.Module.TranslationWriteItemMember})";
                    }
                    else
                    {
                        line = $"(({this.Module.TranslationWriteInterface})(({Module.TranslationItemInterface}){itemAccessor}).{this.Module.TranslationWriteItemMember})";
                    }
                    using (var args = sb.Call( $"{line}.Write{loquiGen.GetGenericTypes(true, MaskType.Normal)}"))
                    {
                        args.Add($"item: {itemAccessor}");
                        args.Add($"writer: {writerAccessor}");

                        var translArgs = new List<string>();

                        if (data?.RecordTypeConverter != null
                            && data.RecordTypeConverter.FromConversions.Count > 0)
                        {
                            translArgs.Add($"{objGen.RegistrationName}.{(typeGen.Name ?? typeGen.Parent?.Name)}Converter");
                        }

                        if (data.OverflowRecordType.HasValue)
                        {
                            translArgs.Add($"RecordTypes.{data.OverflowRecordType}");
                        }

                        if (translArgs.Count > 0)
                        {
                            args.Add($"translationParams: {converterAccessor}.With({string.Join(", ", translArgs)})");
                        }
                        else if (converterAccessor != null)
                        {
                            args.Add($"translationParams: {converterAccessor}");
                        }
                    }
                }
            }
        }
    }

    public bool NeedsHeaderProcessing(LoquiType loquiGen)
    {
        if (!loquiGen.Singleton
            && loquiGen.TargetObjectGeneration != null
            && loquiGen.GetFieldData().HasTrigger
            && !loquiGen.TargetObjectGeneration.Abstract
            && loquiGen.TargetObjectGeneration.GetObjectData().TriggeringSource == null)
        {
            return true;
        }
        return false;
    }

    public override bool ShouldGenerateCopyIn(TypeGeneration typeGen)
    {
        var loquiGen = typeGen as LoquiType;
        return !loquiGen.Singleton || loquiGen.SetterInterfaceType != LoquiInterfaceType.IGetter;
    }

    public override async Task GenerateCopyIn(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor frameAccessor,
        Accessor itemAccessor,
        Accessor errorMaskAccessor,
        Accessor translationMaskAccessor)
    {
        var loqui = typeGen as LoquiType;
        var data = loqui.GetFieldData();
        if (data.MarkerType.HasValue
            && !data.RecordType.HasValue)
        {
            sb.AppendLine($"frame.Position += frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(GameConstants.SubConstants.HeaderLength)} + contentLength; // Skip marker");
        }
        if (loqui.TargetObjectGeneration != null)
        {
            if (loqui.SetterInterfaceType == LoquiInterfaceType.IGetter) return;
            if (loqui.Singleton)
            {
                using (var args = sb.Call(
                           $"{Loqui.Generation.Utility.Await(this.IsAsync(typeGen, read: true))}{itemAccessor}.{this.Module.CopyInFromPrefix}{TranslationTerm}"))
                {
                    args.Add($"frame: {frameAccessor}");
                    args.Add($"translationParams: null");
                }
            }
            else
            {
                if (NeedsHeaderProcessing(loqui))
                {
                    sb.AppendLine($"frame.Position += frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(GameConstants.SubConstants.HeaderLength)}; // Skip header");
                }
                using (var args = sb.Call(
                           $"{itemAccessor} = {loqui.TargetObjectGeneration.Namespace}.{loqui.TypeNameInternal(getter: false, internalInterface: true)}.{this.Module.CreateFromPrefix}{this.Module.ModuleNickname}"))
                {
                    args.Add($"frame: {frameAccessor}");
                    var trans = new List<string>();
                        
                    if (data?.RecordTypeConverter != null
                        && data.RecordTypeConverter.FromConversions.Count > 0)
                    {
                        trans.Add($"{objGen.RegistrationName}.{typeGen.Name}Converter");
                    }

                    if (data.OverflowRecordType.HasValue)
                    {
                        trans.Add($"lastParsed.{nameof(PreviousParse.LengthOverride)}");
                    }
 
                    if (trans.Count > 0)
                    {
                        args.Add($"translationParams: translationParams.With({string.Join(", ", trans)}).DoNotShortCircuit()");
                    }
                    else if (await NeedsRecordTypeConverter(loqui))
                    {
                        args.Add($"translationParams: translationParams.DoNotShortCircuit()");
                    }
                }
            }
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public static async Task<bool> NeedsRecordTypeConverter(LoquiType loqui)
    {
        foreach (var field in loqui.TargetObjectGeneration.IterateFields(includeBaseClass: true))
        {
            if (field.GetFieldData().HasTrigger) return true;
        }
        foreach (var subObj in await loqui.ObjectGen.InheritingObjects())
        {
            if (subObj.GetObjectData().BaseRecordTypeConverter != null) return true;
        }
        return false;
    }

    public override void GenerateCopyInRet(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration targetGen,
        TypeGeneration typeGen,
        Accessor readerAccessor,
        AsyncMode asyncMode,
        Accessor retAccessor,
        Accessor outItemAccessor,
        Accessor errorMaskAccessor,
        Accessor translationAccessor,
        Accessor converterAccessor,
        bool inline)
    {
        LoquiType loqui = typeGen as LoquiType;
        if (inline)
        {
            if (loqui.GenericDef != null)
            {
                sb.AppendLine($"{retAccessor}{Loqui.Generation.Utility.Await(asyncMode)}LoquiBinary{(asyncMode == AsyncMode.Off ? null : "Async")}Translation<{loqui.ObjectTypeName}{loqui.GenericTypes(getter: false)}>.Instance.Parse");
            }
            else
            {
                sb.AppendLine($"{retAccessor}{loqui.ObjectTypeName}{loqui.GenericTypes(getter: false)}.TryCreateFromBinary");
            }
        }
        else
        {
            sb.AppendLine($"var ret = {loqui.ObjectTypeName}{loqui.GenericTypes(getter: false)}.TryCreateFromBinary({readerAccessor}, out var tmp{outItemAccessor}, {converterAccessor});");
            sb.AppendLine($"{outItemAccessor} = tmp{outItemAccessor};");
            sb.AppendLine("return ret;");
        }
    }

    public override async Task GenerateWrapperFields(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor structDataAccessor,  
        Accessor recordDataAccessor, 
        int? currentPosition,
        string passedLengthAccessor,
        DataType dataType)
    {
        LoquiType loqui = typeGen as LoquiType;
        var data = typeGen.GetFieldData();
        switch (data.BinaryOverlayFallback)
        {
            case BinaryGenerationType.Normal:
                break;
            case BinaryGenerationType.NoGeneration:
                return;
            case BinaryGenerationType.Custom:
                await this.Module.CustomLogic.GenerateForCustomFlagWrapperFields(
                    sb,
                    objGen,
                    typeGen,
                    currentPosition,
                    passedLengthAccessor,
                    dataType);
                return;
            default:
                throw new NotImplementedException();
        }

        string DataAccessor(Accessor accessor, string positionStr, string lenStr)
        {
            if (objGen.GetObjectType() == ObjectType.Mod)
            {
                return $"{nameof(PluginBinaryOverlay)}.{nameof(PluginBinaryOverlay.LockExtractMemory)}({accessor}, {positionStr}, {lenStr})";
            }
            else
            {
                return $"{accessor}.Slice({positionStr})";
            }
        }

        string recConverter = $"default({nameof(TypedParseParams)})";
        if (loqui.GetFieldData()?.RecordTypeConverter != null
            && loqui.GetFieldData().RecordTypeConverter.FromConversions.Count > 0)
        {
            recConverter = $"{objGen.RegistrationName}.{loqui.Name}Converter";
        }

        var isRequiredRecord = !loqui.Nullable && data.HasTrigger;
        if (dataType == null)
        {
            if (loqui.GetFieldData()?.HasTrigger ?? false)
            {
                if (loqui is GroupType)
                {
                    sb.AppendLine($"private List<{GetLocationObjectString(objGen)}>? _{typeGen.Name}Locations;");
                    using (sb.Line())
                    {
                        if (loqui.IsNullable)
                        {
                            sb.Append($"public ");
                        }
                        else
                        {
                            sb.Append($"private ");
                        }

                        sb.Append(
                            $"{loqui.Interface(getter: true, internalInterface: true)}{(typeGen.CanBeNullable(getter: true) ? "?" : null)} ");
                        if (!loqui.IsNullable
                            || isRequiredRecord)
                        {
                            sb.Append("_");
                        }

                        sb.Append($"{typeGen.Name}");
                        sb.Append($" => _{typeGen.Name}Locations != null ? ");
                        sb.Append(
                            $"{this.Module.BinaryOverlayClassName(loqui)}.{loqui.TargetObjectGeneration.Name}Factory({recordDataAccessor}, _{typeGen.Name}Locations, _package");
                        if (!recConverter.StartsWith("default("))
                        {
                            sb.Append($", {recConverter}");
                        }

                        sb.Append($") ");
                        sb.Append($": default;");
                    }
                }
                else if (loqui.TargetObjectGeneration.IsTypelessStruct())
                {
                    if (loqui.Singleton
                        || isRequiredRecord)
                    {
                        sb.AppendLine($"private {loqui.Interface(getter: true, internalInterface: true)}? _{typeGen.Name};");
                    }
                    else if (loqui.Nullable)
                    {
                        sb.AppendLine($"public {loqui.Interface(getter: true, internalInterface: true)}? {typeGen.Name} {{ get; private set; }}");
                    }
                }
                else
                {
                    var severalSubTypes = data.GenerationTypes
                        .Select(i => i.Value)
                        .WhereCastable<TypeGeneration, LoquiType>()
                        .Where(loqui => !loqui?.TargetObjectGeneration?.Abstract ?? true)
                        .CountGreaterThan(1);
                    if (severalSubTypes)
                    {
                        sb.AppendLine($"private {nameof(RecordType)} _{typeGen.Name}Type;");
                    }

                    if (data.OverflowRecordType.HasValue)
                    {
                        sb.AppendLine($"private int? _{typeGen.Name}LengthOverride;");
                    }
                    sb.AppendLine($"private {GetLocationObjectString(objGen)}? _{typeGen.Name}Location;");
                    using (sb.Line())
                    {
                        if (loqui.IsNullable)
                        {
                            sb.Append($"public ");
                        }
                        else
                        {
                            sb.Append($"private ");
                        }
                        sb.Append($"{loqui.Interface(getter: true, internalInterface: true)}{(typeGen.CanBeNullable(getter: true) ? "?" : null)} ");
                        if (!loqui.IsNullable
                            || isRequiredRecord)
                        {
                            sb.Append("_");
                        }
                        sb.Append($"{typeGen.Name}");
                        if (!severalSubTypes)
                        {
                            sb.Append($" => _{typeGen.Name}Location.HasValue ? ");
                            sb.Append($"{this.Module.BinaryOverlayClassName(loqui)}.{loqui.TargetObjectGeneration.Name}Factory({DataAccessor(recordDataAccessor, $"_{typeGen.Name}Location!.Value.Min", $"_{typeGen.Name}Location!.Value.Max")}, _package{(data.OverflowRecordType.HasValue ? $", {nameof(TypedParseParams)}.{nameof(TypedParseParams.FromLengthOverride)}(_{typeGen.Name}LengthOverride)" : null)}");
                            if (!recConverter.StartsWith("default("))
                            {
                                sb.Append($", {recConverter}");
                            }
                            sb.Append($") ");
                            sb.Append($": default;");
                        }
                    }
                    if (severalSubTypes)
                    {
                        using (sb.CurlyBrace())
                        {
                            sb.AppendLine("get");
                            using (sb.CurlyBrace())
                            {
                                sb.AppendLine($"if (!_{typeGen.Name}Location.HasValue) return default;");
                                sb.AppendLine($"switch (_{typeGen.Name}Type.TypeInt)");
                                using (sb.CurlyBrace())
                                {
                                    foreach (var gen in data.GenerationTypes)
                                    {
                                        if (!(gen.Value is LoquiType subLoq)) continue;
                                        if (subLoq?.TargetObjectGeneration?.Abstract ?? false) continue;
                                        foreach (var trigger in gen.Key)
                                        {
                                            sb.AppendLine($"case RecordTypeInts.{trigger.Type}:");
                                        }
                                        using (sb.IncreaseDepth())
                                        using (sb.Line())
                                        {
                                            sb.Append($"return {this.Module.BinaryOverlayClassName(subLoq)}.{subLoq.TargetObjectGeneration.Name}Factory({DataAccessor(recordDataAccessor, $"_{subLoq.Name}Location!.Value.Min", $"_{subLoq.Name}Location!.Value.Max")}, _package");
                                            if (!loqui.Singleton)
                                            {
                                                sb.Append($", {recConverter}");
                                            }
                                            sb.Append($");");
                                        }
                                    }
                                    sb.AppendLine("default:");
                                    using (sb.IncreaseDepth())
                                    {
                                        sb.AppendLine("throw new ArgumentException();");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (loqui.Singleton)
                {
                    sb.AppendLine($"private {loqui.Interface(getter: true, internalInterface: true)} _{typeGen.Name} {{ get; private set; }}");
                }
                else
                {
                    var finalPosParam = loqui.TargetObjectGeneration.IsVariableLengthStruct() ? $".Slice(0, {structDataAccessor}.Length - {passedLengthAccessor})" : null; 
                    sb.AppendLine($"public {loqui.Interface(getter: true, internalInterface: true)} {typeGen.Name} => {this.Module.BinaryOverlayClassName(loqui)}.{loqui.TargetObjectGeneration.Name}Factory({structDataAccessor}{(passedLengthAccessor == null ? null : $".Slice({passedLengthAccessor})")}{finalPosParam}, _package, {recConverter});"); 
                }
            }
        }
        else
        {
            isRequiredRecord = true;
            DataBinaryTranslationGeneration.GenerateWrapperExtraMembers(sb, dataType, objGen, typeGen, passedLengthAccessor);
            var finalPosParam = loqui.TargetObjectGeneration.IsVariableLengthStruct() ? $".Slice(0, _{dataType.GetFieldData().RecordType}Location!.Value.Width - {currentPosition})" : null; 
            sb.AppendLine($"private {loqui.Interface(getter: true, internalInterface: true)}? _{typeGen.Name} => _{typeGen.Name}_IsSet ? {this.Module.BinaryOverlayClassName(loqui)}.{loqui.TargetObjectGeneration.Name}Factory({DataAccessor(recordDataAccessor, $"_{typeGen.Name}Location", null)}{finalPosParam}, _package) : default;"); 
        }

        if (loqui.Singleton
            || isRequiredRecord)
        {
            sb.AppendLine($"public {loqui.Interface(getter: true, internalInterface: true)} {typeGen.Name} => _{typeGen.Name} ?? new {loqui.DirectTypeName}({(loqui.ThisConstruction ? "this" : null)});");
        }
    }

    public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
    {
        var data = typeGen.GetFieldData();
        if (data.Length.HasValue) return data.Length;

        LoquiType loqui = typeGen as LoquiType;
        if (loqui.TargetObjectGeneration == null) return null;

        var sum = 0;
        foreach (var item in loqui.TargetObjectGeneration.IterateFields(includeBaseClass: true))
        {
            if (item.Nullable) return null;
            if (!this.Module.TryGetTypeGeneration(item.GetType(), out var gen)) continue;
            if (item.GetFieldData().Binary == BinaryGenerationType.NoGeneration) continue;
            var len = await gen.ExpectedLength(loqui.TargetObjectGeneration, item);
            if (len == null) return null;
            sum += len.Value;
        }

        if (loqui.TargetObjectGeneration.Abstract)
        {
            int? absSum = null;
            foreach (var inheritingObj in await loqui.TargetObjectGeneration.InheritingObjects())
            {
                int objectSum = 0;
                foreach (var item in inheritingObj.IterateFields(includeBaseClass: true))
                {
                    if (item.Nullable) return null;
                    if (!this.Module.TryGetTypeGeneration(item.GetType(), out var gen)) continue;
                    var len = await gen.ExpectedLength(inheritingObj, item);
                    if (len == null) return null;
                    objectSum += len.Value;
                }
                if (absSum == null)
                {
                    absSum = objectSum;
                }
                else if (absSum.Value != objectSum)
                {
                    // Inheriting objects don't agree on their length, so we can't expect a certain length 
                    return null;
                }
            }
            if (absSum == null) return null;
            sum += absSum.Value;
        }

        return sum;
    }

    private string GetLocationObjectString(ObjectGeneration obj) => obj.GetObjectType() == ObjectType.Mod ? nameof(RangeInt64) : nameof(RangeInt32);

    public override async Task GenerateWrapperRecordTypeParse(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor locationAccessor,
        Accessor packageAccessor,
        Accessor converterAccessor)
    {
        LoquiType loqui = typeGen as LoquiType;
        var data = loqui.GetFieldData();

        switch (data.BinaryOverlayFallback)
        {
            case BinaryGenerationType.Normal:
                break;
            case BinaryGenerationType.NoGeneration:
                return;
            case BinaryGenerationType.Custom:
                using (var args = sb.Call(
                           $"{typeGen.Name}CustomParse"))
                {
                    args.Add("stream");
                    args.Add("finalPos");
                    args.Add("offset");
                }
                return;
            default:
                throw new NotImplementedException();
        }

        string accessor;
        if (loqui.Singleton
            || !loqui.Nullable)
        {
            accessor = $"_{typeGen.Name}";
        }
        else
        {
            accessor = typeGen.Name;
        }
        if (data.MarkerType.HasValue)
        {
            sb.AppendLine($"stream.Position += {packageAccessor}.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.SubConstants.HeaderLength; // Skip marker");
        }

        if (loqui.TargetObjectGeneration.IsTopLevelGroup())
        {
            sb.AppendLine($"_{typeGen.Name}Locations ??= new();");
            sb.AppendLine($"_{typeGen.Name}Locations.Add(new {GetLocationObjectString(objGen)}({locationAccessor}, finalPos - offset));");
        }
        else if (!loqui.TargetObjectGeneration.IsTypelessStruct() && (loqui.GetFieldData()?.HasTrigger ?? false))
        {
            sb.AppendLine($"_{typeGen.Name}Location = new {GetLocationObjectString(objGen)}({locationAccessor}, finalPos - offset);");
            var severalSubTypes = data.GenerationTypes
                .Select(i => i.Value)
                .WhereCastable<TypeGeneration, LoquiType>()
                .Where(loqui => !loqui?.TargetObjectGeneration?.Abstract ?? true)
                .CountGreaterThan(1);
            if (severalSubTypes)
            {
                sb.AppendLine($"_{typeGen.Name}Type = type;");
            }
            if (data.OverflowRecordType.HasValue
                && data.BinaryOverlayFallback != BinaryGenerationType.Custom)
            {
                sb.AppendLine($"_{typeGen.Name}LengthOverride = lastParsed.{nameof(PreviousParse.LengthOverride)};");
                sb.AppendLine($"if (lastParsed.{nameof(PreviousParse.LengthOverride)}.HasValue)");
                using (sb.CurlyBrace())
                {
                    sb.AppendLine($"stream.Position += lastParsed.{nameof(PreviousParse.LengthOverride)}.Value;");
                }
            }
        }
        else
        {
            if (NeedsHeaderProcessing(loqui))
            {
                sb.AppendLine($"stream.Position += _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.SubConstants.HeaderLength;");
            }
            using (var args = sb.Call(
                       $"this.{accessor} = {this.Module.BinaryOverlayClassName(loqui)}.{loqui.TargetObjectGeneration.Name}Factory"))
            {
                args.Add($"stream: stream");
                args.Add($"package: {packageAccessor}");
                if (loqui.TargetObjectGeneration.IsVariableLengthStruct())
                {
                    args.AddPassArg($"finalPos");
                }
                args.Add($"translationParams: {converterAccessor}.DoNotShortCircuit()");
            }
        }
    }

    public override string GenerateForTypicalWrapper(
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor dataAccessor,
        Accessor packageAccessor)
    {
        LoquiType loqui = typeGen as LoquiType;
        return $"{this.Module.BinaryOverlayClass(loqui.TargetObjectGeneration)}.{loqui.TargetObjectGeneration.Name}Factory({dataAccessor}, {packageAccessor})";
    }
}