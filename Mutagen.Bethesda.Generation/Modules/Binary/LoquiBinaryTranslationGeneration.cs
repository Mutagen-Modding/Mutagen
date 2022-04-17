using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mutagen.Bethesda.Generation.Fields;

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
        FileGeneration fg,
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
            fg.AppendLine($"if ({itemAccessor} is {{}} {typeGen.Name}Item)");
            itemAccessor = $"{typeGen.Name}Item";
        }
        else
        {
            // We want to cache retrievals, in case it's a wrapper being written 
            fg.AppendLine($"var {typeGen.Name}Item = {itemAccessor};");
            itemAccessor = $"{typeGen.Name}Item";
        }
        using (new BraceWrapper(fg, doIt: typeGen.Nullable))
        {
            if (isGroup)
            {
                var dictGroup = loquiGen.TargetObjectGeneration.Name == $"{objGen.ProtoGen.Protocol.Namespace}Group";
                fg.AppendLine($"if ({itemAccessor}.{(dictGroup ? "RecordCache" : "Records")}.Count > 0)");
            }
            using (new BraceWrapper(fg, doIt: isGroup))
            {
                var data = loquiGen.GetFieldData();
                    
                if (data.MarkerType.HasValue)
                {
                    fg.AppendLine($"using ({nameof(HeaderExport)}.{nameof(HeaderExport.Subrecord)}(writer, {objGen.RecordTypeHeaderName(data.MarkerType.Value)})) {{ }}");
                }
                var needsHeaderWrite = false;
                if (NeedsHeaderProcessing(loquiGen))
                {
                    needsHeaderWrite = true;
                    fg.AppendLine($"using ({nameof(HeaderExport)}.{nameof(HeaderExport.Subrecord)}(writer, {loquiGen.GetFieldData().TriggeringRecordAccessor}))");
                }
                using (new BraceWrapper(fg, doIt: needsHeaderWrite))
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
                    using (var args = new ArgsWrapper(fg, $"{line}.Write{loquiGen.GetGenericTypes(true, MaskType.Normal)}"))
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
        FileGeneration fg,
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
            fg.AppendLine($"frame.Position += frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(GameConstants.SubConstants.HeaderLength)} + contentLength; // Skip marker");
        }
        if (loqui.TargetObjectGeneration != null)
        {
            if (loqui.SetterInterfaceType == LoquiInterfaceType.IGetter) return;
            if (loqui.Singleton)
            {
                using (var args = new ArgsWrapper(fg,
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
                    fg.AppendLine($"frame.Position += frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(GameConstants.SubConstants.HeaderLength)}; // Skip header");
                }
                using (var args = new ArgsWrapper(fg,
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
                        args.Add($"translationParams: translationParams.With({string.Join(", ", trans)})");
                    }
                    else if (await NeedsRecordTypeConverter(loqui))
                    {
                        args.AddPassArg($"translationParams");
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
        FileGeneration fg,
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
                fg.AppendLine($"{retAccessor}{Loqui.Generation.Utility.Await(asyncMode)}LoquiBinary{(asyncMode == AsyncMode.Off ? null : "Async")}Translation<{loqui.ObjectTypeName}{loqui.GenericTypes(getter: false)}>.Instance.Parse");
            }
            else
            {
                fg.AppendLine($"{retAccessor}{loqui.ObjectTypeName}{loqui.GenericTypes(getter: false)}.TryCreateFromBinary");
            }
        }
        else
        {
            fg.AppendLine($"var ret = {loqui.ObjectTypeName}{loqui.GenericTypes(getter: false)}.TryCreateFromBinary({readerAccessor}, out var tmp{outItemAccessor}, {converterAccessor});");
            fg.AppendLine($"{outItemAccessor} = tmp{outItemAccessor};");
            fg.AppendLine("return ret;");
        }
    }

    public override async Task GenerateWrapperFields(
        FileGeneration fg,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor dataAccessor,
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
                    fg,
                    objGen,
                    typeGen,
                    dataAccessor,
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
                    fg.AppendLine($"private List<{GetLocationObjectString(objGen)}>? _{typeGen.Name}Locations;");
                    using (new LineWrapper(fg))
                    {
                        if (loqui.IsNullable)
                        {
                            fg.Append($"public ");
                        }
                        else
                        {
                            fg.Append($"private ");
                        }

                        fg.Append(
                            $"{loqui.Interface(getter: true, internalInterface: true)}{(typeGen.CanBeNullable(getter: true) ? "?" : null)} ");
                        if (!loqui.IsNullable
                            || isRequiredRecord)
                        {
                            fg.Append("_");
                        }

                        fg.Append($"{typeGen.Name}");
                        fg.Append($" => _{typeGen.Name}Locations != null ? ");
                        fg.Append(
                            $"{this.Module.BinaryOverlayClassName(loqui)}.{loqui.TargetObjectGeneration.Name}Factory(_data, _{typeGen.Name}Locations, _package");
                        if (!recConverter.StartsWith("default("))
                        {
                            fg.Append($", {recConverter}");
                        }

                        fg.Append($") ");
                        fg.Append($": default;");
                    }
                }
                else if (loqui.TargetObjectGeneration.IsTypelessStruct())
                {
                    if (loqui.Singleton
                        || isRequiredRecord)
                    {
                        fg.AppendLine($"private {loqui.Interface(getter: true, internalInterface: true)}? _{typeGen.Name};");
                    }
                    else if (loqui.Nullable)
                    {
                        fg.AppendLine($"public {loqui.Interface(getter: true, internalInterface: true)}? {typeGen.Name} {{ get; private set; }}");
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
                        fg.AppendLine($"private {nameof(RecordType)} _{typeGen.Name}Type;");
                    }

                    if (data.OverflowRecordType.HasValue)
                    {
                        fg.AppendLine($"private int? _{typeGen.Name}LengthOverride;");
                    }
                    fg.AppendLine($"private {GetLocationObjectString(objGen)}? _{typeGen.Name}Location;");
                    using (new LineWrapper(fg))
                    {
                        if (loqui.IsNullable)
                        {
                            fg.Append($"public ");
                        }
                        else
                        {
                            fg.Append($"private ");
                        }
                        fg.Append($"{loqui.Interface(getter: true, internalInterface: true)}{(typeGen.CanBeNullable(getter: true) ? "?" : null)} ");
                        if (!loqui.IsNullable
                            || isRequiredRecord)
                        {
                            fg.Append("_");
                        }
                        fg.Append($"{typeGen.Name}");
                        if (!severalSubTypes)
                        {
                            fg.Append($" => _{typeGen.Name}Location.HasValue ? ");
                            fg.Append($"{this.Module.BinaryOverlayClassName(loqui)}.{loqui.TargetObjectGeneration.Name}Factory(new {nameof(OverlayStream)}({DataAccessor(dataAccessor, $"_{typeGen.Name}Location!.Value.Min", $"_{typeGen.Name}Location!.Value.Max")}, _package), _package{(data.OverflowRecordType.HasValue ? $", new {nameof(TypedParseParams)}(_{typeGen.Name}LengthOverride, null)" : null)}");
                            if (!recConverter.StartsWith("default("))
                            {
                                fg.Append($", {recConverter}");
                            }
                            fg.Append($") ");
                            fg.Append($": default;");
                        }
                    }
                    if (severalSubTypes)
                    {
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine("get");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine($"if (!_{typeGen.Name}Location.HasValue) return default;");
                                fg.AppendLine($"switch (_{typeGen.Name}Type.TypeInt)");
                                using (new BraceWrapper(fg))
                                {
                                    foreach (var gen in data.GenerationTypes)
                                    {
                                        if (!(gen.Value is LoquiType subLoq)) continue;
                                        if (subLoq?.TargetObjectGeneration?.Abstract ?? false) continue;
                                        foreach (var trigger in gen.Key)
                                        {
                                            fg.AppendLine($"case 0x{trigger.TypeInt.ToString("X")}: // {trigger.Type}");
                                        }
                                        using (new DepthWrapper(fg))
                                        using (new LineWrapper(fg))
                                        {
                                            fg.Append($"return {this.Module.BinaryOverlayClassName(subLoq)}.{subLoq.TargetObjectGeneration.Name}Factory(new {nameof(OverlayStream)}({DataAccessor(dataAccessor, $"_{subLoq.Name}Location!.Value.Min", $"_{subLoq.Name}Location!.Value.Max")}, _package), _package");
                                            if (!loqui.Singleton)
                                            {
                                                fg.Append($", {recConverter}");
                                            }
                                            fg.Append($");");
                                        }
                                    }
                                    fg.AppendLine("default:");
                                    using (new DepthWrapper(fg))
                                    {
                                        fg.AppendLine("throw new ArgumentException();");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (!loqui.Singleton)
                {
                    fg.AppendLine($"public {loqui.Interface(getter: true, internalInterface: true)} {typeGen.Name} => {this.Module.BinaryOverlayClassName(loqui)}.{loqui.TargetObjectGeneration.Name}Factory(new {nameof(OverlayStream)}({dataAccessor}.Slice({passedLengthAccessor ?? "0x0"}), _package), _package, {recConverter});");
                }
                else
                {
                    fg.AppendLine($"private {loqui.Interface(getter: true, internalInterface: true)} _{typeGen.Name} {{ get; private set; }}");
                }
            }
        }
        else
        {
            isRequiredRecord = true;
            DataBinaryTranslationGeneration.GenerateWrapperExtraMembers(fg, dataType, objGen, typeGen, passedLengthAccessor);
            var finalPosParam = loqui.TargetObjectGeneration.IsVariableLengthStruct() ? $", _{dataType.GetFieldData().RecordType}Location!.Value.Width - {currentPosition}" : null;
            fg.AppendLine($"private {loqui.Interface(getter: true, internalInterface: true)}? _{typeGen.Name} => _{typeGen.Name}_IsSet ? {this.Module.BinaryOverlayClassName(loqui)}.{loqui.TargetObjectGeneration.Name}Factory(new {nameof(OverlayStream)}({DataAccessor(dataAccessor, $"_{typeGen.Name}Location", null)}, _package), _package{finalPosParam}) : default;");
        }

        if (loqui.Singleton
            || isRequiredRecord)
        {
            fg.AppendLine($"public {loqui.Interface(getter: true, internalInterface: true)} {typeGen.Name} => _{typeGen.Name} ?? new {loqui.DirectTypeName}({(loqui.ThisConstruction ? "this" : null)});");
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
        FileGeneration fg,
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
                using (var args = new ArgsWrapper(fg,
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
            fg.AppendLine($"stream.Position += {packageAccessor}.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.SubConstants.HeaderLength; // Skip marker");
        }

        if (loqui.TargetObjectGeneration.IsTopLevelGroup())
        {
            fg.AppendLine($"_{typeGen.Name}Locations ??= new();");
            fg.AppendLine($"_{typeGen.Name}Locations.Add(new {GetLocationObjectString(objGen)}({locationAccessor}, finalPos - offset));");
        }
        else if (!loqui.TargetObjectGeneration.IsTypelessStruct() && (loqui.GetFieldData()?.HasTrigger ?? false))
        {
            fg.AppendLine($"_{typeGen.Name}Location = new {GetLocationObjectString(objGen)}({locationAccessor}, finalPos - offset);");
            var severalSubTypes = data.GenerationTypes
                .Select(i => i.Value)
                .WhereCastable<TypeGeneration, LoquiType>()
                .Where(loqui => !loqui?.TargetObjectGeneration?.Abstract ?? true)
                .CountGreaterThan(1);
            if (severalSubTypes)
            {
                fg.AppendLine($"_{typeGen.Name}Type = type;");
            }
            if (data.OverflowRecordType.HasValue
                && data.BinaryOverlayFallback != BinaryGenerationType.Custom)
            {
                fg.AppendLine($"_{typeGen.Name}LengthOverride = lastParsed.{nameof(PreviousParse.LengthOverride)};");
                fg.AppendLine($"if (lastParsed.{nameof(PreviousParse.LengthOverride)}.HasValue)");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"stream.Position += lastParsed.{nameof(PreviousParse.LengthOverride)}.Value;");
                }
            }
        }
        else
        {
            if (NeedsHeaderProcessing(loqui))
            {
                fg.AppendLine($"stream.Position += _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.SubConstants.HeaderLength;");
            }
            using (var args = new ArgsWrapper(fg,
                       $"this.{accessor} = {this.Module.BinaryOverlayClassName(loqui)}.{loqui.TargetObjectGeneration.Name}Factory"))
            {
                args.Add($"stream: stream");
                args.Add($"package: {packageAccessor}");
                if (loqui.TargetObjectGeneration.IsVariableLengthStruct())
                {
                    args.AddPassArg($"finalPos");
                }
                args.Add($"parseParams: {converterAccessor}");
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