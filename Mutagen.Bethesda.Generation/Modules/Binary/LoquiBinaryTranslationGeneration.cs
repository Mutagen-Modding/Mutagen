using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class LoquiBinaryTranslationGeneration : BinaryTranslationGeneration
    {
        public const string AsyncOverrideKey = "AsyncOverride";

        public string ModNickname;
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

        public override bool AllowDirectParse(ObjectGeneration objGen, TypeGeneration typeGen, bool squashedRepeatedList)
        {
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

        public LoquiBinaryTranslationGeneration(string modNickname)
        {
            this.ModNickname = modNickname;
        }

        public override void GenerateWrite(
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
            if (typeGen.HasBeenSet)
            {
                fg.AppendLine($"if ({itemAccessor.DirectAccess}.TryGet(out var {typeGen.Name}Item))");
                itemAccessor = $"{typeGen.Name}Item";
            }
            else
            {
                // We want to cache retrievals, in case it's a wrapper being written 
                fg.AppendLine($"var {typeGen.Name}Item = {itemAccessor.DirectAccess};");
                itemAccessor = $"{typeGen.Name}Item";
            }
            using (new BraceWrapper(fg, doIt: typeGen.HasBeenSet))
            {
                if (isGroup)
                {
                    var dictGroup = loquiGen.TargetObjectGeneration.Name == "Group";
                    fg.AppendLine($"if ({itemAccessor.PropertyOrDirectAccess}.{(dictGroup ? "RecordCache" : "Records")}.Count > 0)");
                }
                using (new BraceWrapper(fg, doIt: isGroup))
                {
                    if (loquiGen.TryGetFieldData(out var data)
                        && data.MarkerType.HasValue)
                    {
                        fg.AppendLine($"using (HeaderExport.ExportHeader(writer, {objGen.RegistrationName}.{data.MarkerType.Value.Type}_HEADER, Mutagen.Bethesda.Binary.ObjectType.Subrecord)) {{ }}");
                    }
                    var needsHeaderWrite = false;
                    if (NeedsHeaderProcessing(loquiGen))
                    {
                        needsHeaderWrite = true;
                        fg.AppendLine($"using (HeaderExport.ExportHeader(writer, {loquiGen.GetFieldData().TriggeringRecordSetAccessor}, Mutagen.Bethesda.Binary.ObjectType.Subrecord))");
                    }
                    using (new BraceWrapper(fg, doIt: needsHeaderWrite))
                    {
                        string line;
                        if (loquiGen.TargetObjectGeneration != null)
                        {
                            line = $"(({this.Module.TranslationWriteClassName(loquiGen.TargetObjectGeneration)})(({nameof(IBinaryItem)}){itemAccessor}).{this.Module.TranslationWriteItemMember})";
                        }
                        else
                        {
                            line = $"(({this.Module.TranslationWriteInterface})(({nameof(IBinaryItem)}){itemAccessor}).{this.Module.TranslationWriteItemMember})";
                        }
                        using (var args = new ArgsWrapper(fg, $"{line}.Write{loquiGen.GetGenericTypes(true, MaskType.Normal)}"))
                        {
                            args.Add($"item: {itemAccessor}");
                            args.Add($"writer: {writerAccessor}");
                            if (data?.RecordTypeConverter != null
                                && data.RecordTypeConverter.FromConversions.Count > 0)
                            {
                                args.Add($"recordTypeConverter: {objGen.RegistrationName}.{(typeGen.Name ?? typeGen.Parent?.Name)}Converter");
                            }
                            else if (converterAccessor != null)
                            {
                                args.Add($"recordTypeConverter: {converterAccessor}");
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
            if (loqui.TryGetFieldData(out var data)
                && data.MarkerType.HasValue
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
                        $"{Loqui.Generation.Utility.Await(this.IsAsync(typeGen, read: true))}{itemAccessor.DirectAccess}.{this.Module.CopyInFromPrefix}{ModNickname}"))
                    {
                        args.Add($"frame: {frameAccessor}");
                        args.Add($"recordTypeConverter: null");
                    }
                    if (loqui.Name == "ModHeader")
                    {
                        fg.AppendLine($"{frameAccessor}.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.MasterReferences)}!.SetTo(item.ModHeader.MasterReferences);");
                    }
                }
                else
                {
                    if (NeedsHeaderProcessing(loqui))
                    {
                        fg.AppendLine($"frame.Position += frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(GameConstants.SubConstants.HeaderLength)}; // Skip header");
                    }
                    using (var args = new ArgsWrapper(fg,
                        $"{itemAccessor.DirectAccess} = {loqui.TargetObjectGeneration.Namespace}.{loqui.TypeNameInternal(getter: false, internalInterface: true)}.{this.Module.CreateFromPrefix}{this.Module.ModuleNickname}"))
                    {
                        args.Add($"frame: {frameAccessor}");
                        if (data?.RecordTypeConverter != null
                            && data.RecordTypeConverter.FromConversions.Count > 0)
                        {
                            args.Add($"recordTypeConverter: {objGen.RegistrationName}.{typeGen.Name}Converter");
                        }
                        else if (await NeedsRecordTypeConverter(loqui))
                        {
                            args.AddPassArg($"recordTypeConverter");
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
            Accessor converterAccessor)
        {
            var targetLoquiGen = targetGen as LoquiType;
            var loquiGen = typeGen as LoquiType;
            var data = loquiGen.GetFieldData();
            asyncMode = this.IsAsync(typeGen, read: true) ? asyncMode : AsyncMode.Off;
            using (var args = new ArgsWrapper(fg,
                $"{retAccessor}{Loqui.Generation.Utility.Await(asyncMode)}LoquiBinary{(asyncMode == AsyncMode.Off ? null : "Async")}Translation<{loquiGen.ObjectTypeName}{loquiGen.GenericTypes(getter: false)}>.Instance.Parse",
                suffixLine: Loqui.Generation.Utility.ConfigAwait(asyncMode)))
            {
                args.Add($"frame: {readerAccessor}");
                if (asyncMode == AsyncMode.Off)
                {
                    args.Add($"item: out {outItemAccessor.DirectAccess}!");
                }
                if (converterAccessor != null
                    && loquiGen.NeedMasters())
                {
                    args.Add($"recordTypeConverter: {converterAccessor}");
                }
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
                    return $"{nameof(BinaryOverlay)}.{nameof(BinaryOverlay.LockExtractMemory)}({accessor}, {positionStr}, {lenStr})";
                }
                else
                {
                    return $"{accessor}.Slice({positionStr})";
                }
            }

            string recConverter = $"default({nameof(RecordTypeConverter)})";
            if (loqui.GetFieldData()?.RecordTypeConverter != null
                && loqui.GetFieldData().RecordTypeConverter.FromConversions.Count > 0)
            {
                recConverter = $"{objGen.RegistrationName}.{loqui.Name}Converter";
            }

            var isRequiredRecord = !loqui.HasBeenSet && data.HasTrigger;
            if (dataType == null)
            {
                if (loqui.GetFieldData()?.HasTrigger ?? false)
                {
                    if (loqui.TargetObjectGeneration.IsTypelessStruct())
                    {
                        if (loqui.Singleton
                            || isRequiredRecord)
                        {
                            fg.AppendLine($"private {loqui.Interface(getter: true, internalInterface: true)}? _{typeGen.Name};");
                        }
                        else if (loqui.HasBeenSet)
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
                                fg.Append($"{this.Module.BinaryOverlayClassName(loqui)}.{loqui.TargetObjectGeneration.Name}Factory(new {nameof(BinaryMemoryReadStream)}({DataAccessor(dataAccessor, $"_{typeGen.Name}Location!.Value.Min", $"_{typeGen.Name}Location!.Value.Max")}), _package");
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
                                                fg.Append($"return {this.Module.BinaryOverlayClassName(subLoq)}.{subLoq.TargetObjectGeneration.Name}Factory(new {nameof(BinaryMemoryReadStream)}({DataAccessor(dataAccessor, $"_{subLoq.Name}Location!.Value.Min", $"_{subLoq.Name}Location!.Value.Max")}), _package");
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
                        fg.AppendLine($"public {loqui.Interface(getter: true, internalInterface: true)} {typeGen.Name} => {this.Module.BinaryOverlayClassName(loqui)}.{loqui.TargetObjectGeneration.Name}Factory(new {nameof(BinaryMemoryReadStream)}({dataAccessor}.Slice({passedLengthAccessor ?? "0x0"})), _package, {recConverter});");
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
                DataBinaryTranslationGeneration.GenerateWrapperExtraMembers(fg, dataType, objGen, typeGen, $"0x{currentPosition:X}");
                fg.AppendLine($"private {loqui.Interface(getter: true, internalInterface: true)}? _{typeGen.Name} => _{typeGen.Name}_IsSet ? {this.Module.BinaryOverlayClassName(loqui)}.{loqui.TargetObjectGeneration.Name}Factory(new {nameof(BinaryMemoryReadStream)}({DataAccessor(dataAccessor, $"_{typeGen.Name}Location", null)}), _package) : default;");
            }

            if (loqui.Singleton
                || isRequiredRecord)
            {
                fg.AppendLine($"public {loqui.Interface(getter: true, internalInterface: true)} {typeGen.Name} => _{typeGen.Name} ?? new {loqui.DirectTypeName}({(loqui.ThisConstruction ? "this" : null)});");
            }
            else if (loqui.HasBeenSet && !loqui.TargetObjectGeneration.IsTypelessStruct())
            {
                fg.AppendLine($"public bool {typeGen.Name}_IsSet => _{typeGen.Name}Location.HasValue;");
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
                if (item.HasBeenSet) return null;
                if (!this.Module.TryGetTypeGeneration(item.GetType(), out var gen)) continue;
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
                        if (item.HasBeenSet) return null;
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
                || !loqui.HasBeenSet)
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

            if (!loqui.TargetObjectGeneration.IsTypelessStruct() && (loqui.GetFieldData()?.HasTrigger ?? false))
            {
                fg.AppendLine($"_{typeGen.Name}Location = new {GetLocationObjectString(objGen)}({locationAccessor}, finalPos);");
                var severalSubTypes = data.GenerationTypes
                    .Select(i => i.Value)
                    .WhereCastable<TypeGeneration, LoquiType>()
                    .Where(loqui => !loqui?.TargetObjectGeneration?.Abstract ?? true)
                    .CountGreaterThan(1);
                if (severalSubTypes)
                {
                    fg.AppendLine($"_{typeGen.Name}Type = type;");
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
                    args.Add($"recordTypeConverter: {converterAccessor}");
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
}
