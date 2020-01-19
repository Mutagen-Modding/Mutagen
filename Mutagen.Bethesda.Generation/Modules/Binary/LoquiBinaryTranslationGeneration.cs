using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Binary;
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
            Accessor translationMaskAccessor)
        {
            var loquiGen = typeGen as LoquiType;
            if (loquiGen.TryGetFieldData(out var data)
                && data.MarkerType.HasValue)
            {
                fg.AppendLine($"using (HeaderExport.ExportHeader(writer, {objGen.RegistrationName}.{data.MarkerType.Value.Type}_HEADER, ObjectType.Subrecord)) {{ }}");
            }
            bool isGroup = objGen.GetObjectType() == ObjectType.Mod
                && loquiGen.TargetObjectGeneration.GetObjectData().ObjectType == ObjectType.Group;
            if (isGroup)
            {
                var dictGroup = loquiGen.TargetObjectGeneration.Name == "Group";
                fg.AppendLine($"if ({itemAccessor.PropertyOrDirectAccess}.{(dictGroup ? "RecordCache" : "Records")}.Count > 0)");
            }
            using (new BraceWrapper(fg, doIt: isGroup || (!this.Module.TranslationMaskParameter && !typeGen.HasBeenSet)))
            {
                // We want to cache retrievals, in case it's a wrapper being written
                fg.AppendLine($"var loquiItem = {itemAccessor.DirectAccess};");
                string line;
                if (loquiGen.TargetObjectGeneration != null)
                {
                    line = $"(({this.Module.TranslationWriteClassName(loquiGen.TargetObjectGeneration)})(({nameof(IBinaryItem)})loquiItem).{this.Module.TranslationWriteItemMember})";
                }
                else
                {
                    line = $"(({this.Module.TranslationWriteInterface})(({nameof(IBinaryItem)})loquiItem).{this.Module.TranslationWriteItemMember})";
                }
                using (var args = new ArgsWrapper(fg, $"{line}.Write{loquiGen.GetGenericTypes(true, MaskType.Normal)}"))
                {
                    args.Add($"item: loquiItem");
                    args.Add($"writer: {writerAccessor}");
                    args.Add($"masterReferences: masterReferences");
                    if (data?.RecordTypeConverter != null
                        && data.RecordTypeConverter.FromConversions.Count > 0)
                    {
                        args.Add($"recordTypeConverter: {objGen.RegistrationName}.{typeGen.Name}Converter");
                    }
                    else
                    {
                        args.Add($"recordTypeConverter: null");
                    }
                }
            }
        }

        public override bool ShouldGenerateCopyIn(TypeGeneration typeGen)
        {
            var loquiGen = typeGen as LoquiType;
            return loquiGen.SingletonType != SingletonLevel.Singleton || loquiGen.SetterInterfaceType != LoquiInterfaceType.IGetter;
        }

        public override void GenerateCopyIn(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor frameAccessor,
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor)
        {
            var loquiGen = typeGen as LoquiType;
            if (loquiGen.TargetObjectGeneration != null)
            {
                if (loquiGen.TryGetFieldData(out var data)
                    && data.MarkerType.HasValue)
                {
                    fg.AppendLine($"frame.Position += frame.{nameof(MutagenFrame.MetaData)}.{nameof(MetaDataConstants.SubConstants)}.{nameof(MetaDataConstants.SubConstants.HeaderLength)} + contentLength; // Skip marker");
                }

                if (loquiGen.SetterInterfaceType == LoquiInterfaceType.IGetter) return;
                if (loquiGen.SingletonType == SingletonLevel.Singleton)
                {
                    using (var args = new ArgsWrapper(fg,
                        $"{Loqui.Generation.Utility.Await(this.IsAsync(typeGen, read: true))}{itemAccessor.DirectAccess}.{this.Module.CopyInFromPrefix}{ModNickname}"))
                    {
                        args.Add($"frame: {frameAccessor}");
                        args.Add($"recordTypeConverter: null");
                        args.Add($"masterReferences: masterReferences");
                    }
                }
                else
                {
                    using (var args = new ArgsWrapper(fg,
                        $"{itemAccessor.DirectAccess} = {loquiGen.TargetObjectGeneration.Namespace}.{loquiGen.ObjectTypeName}.{this.Module.CreateFromPrefix}{this.Module.ModuleNickname}"))
                    {
                        args.Add($"frame: {frameAccessor}");
                        if (data?.RecordTypeConverter != null
                            && data.RecordTypeConverter.FromConversions.Count > 0)
                        {
                            args.Add($"recordTypeConverter: {objGen.RegistrationName}.{typeGen.Name}Converter");
                        }
                        else
                        {
                            args.Add("recordTypeConverter: null");
                        }
                        args.Add($"masterReferences: masterReferences");
                    }
                }
            }
            else
            {
                throw new NotImplementedException();
            }
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
            Accessor translationAccessor)
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
                    args.Add($"item: out {outItemAccessor.DirectAccess}");
                }
                if (objGen.GetObjectType() == ObjectType.Mod)
                {
                    args.Add($"masterReferences: item.TES4.MasterReferences");
                }
                else
                {
                    args.Add($"masterReferences: masterReferences");
                }
                if (data?.RecordTypeConverter != null
                    && data.RecordTypeConverter.FromConversions.Count > 0)
                {
                    args.Add($"recordTypeConverter: {objGen.RegistrationName}.{typeGen.Name}Converter");
                }
            }
        }

        public override void GenerateWrapperFields(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor dataAccessor,
            int? currentPosition,
            DataType dataType)
        {
            LoquiType loqui = typeGen as LoquiType;
            var data = typeGen.GetFieldData();
            switch (data.BinaryOverlayFallback)
            {
                case BinaryGenerationType.Normal:
                    break;
                case BinaryGenerationType.DoNothing:
                case BinaryGenerationType.NoGeneration:
                    return;
                case BinaryGenerationType.Custom:
                    this.Module.CustomLogic.GenerateForCustomFlagWrapperFields(
                        fg,
                        objGen,
                        typeGen,
                        dataAccessor,
                        ref currentPosition,
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

            if (dataType == null)
            {
                if (loqui.GetFieldData()?.HasTrigger ?? false)
                {
                    if (loqui.TargetObjectGeneration.IsTypelessStruct())
                    {
                        if (loqui.SingletonType != SingletonLevel.None)
                        {
                            fg.AppendLine($"private {loqui.Interface(getter: true, internalInterface: true)} _{typeGen.Name};");
                            fg.AppendLine($"public bool {typeGen.Name}_IsSet => true;");
                        }
                        else
                        {
                            fg.AppendLine($"public {loqui.Interface(getter: true, internalInterface: true)} {typeGen.Name} {{ get; private set; }}");
                            if (typeGen.HasBeenSet)
                            {
                                fg.AppendLine($"public bool {typeGen.Name}_IsSet => {typeGen.Name} != null;");
                            }
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
                        fg.AppendLine($"private bool _{typeGen.Name}_IsSet => _{typeGen.Name}Location.HasValue;");
                        using (new LineWrapper(fg))
                        {
                            if (loqui.SingletonType == SingletonLevel.None)
                            {
                                fg.Append($"public ");
                            }
                            else
                            {
                                fg.Append($"private ");
                            }
                            fg.Append($"{loqui.Interface(getter: true, internalInterface: true)} ");
                            if (loqui.SingletonType != SingletonLevel.None)
                            {
                                fg.Append("_");
                            }
                            fg.Append($"{typeGen.Name}");
                            if (!severalSubTypes)
                            {
                                fg.Append($" => _{typeGen.Name}_IsSet ? ");
                                fg.Append($"{this.Module.BinaryOverlayClassName(loqui)}.{loqui.TargetObjectGeneration.Name}Factory(new {nameof(BinaryMemoryReadStream)}({DataAccessor(dataAccessor, $"_{typeGen.Name}Location.Value.Min", $"_{typeGen.Name}Location.Value.Max")}), _package");
                                if (loqui.SingletonType == SingletonLevel.None)
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
                                    fg.AppendLine($"if (!_{typeGen.Name}_IsSet) return default;");
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
                                                fg.Append($"return {this.Module.BinaryOverlayClassName(subLoq)}.{subLoq.TargetObjectGeneration.Name}Factory(new {nameof(BinaryMemoryReadStream)}({DataAccessor(dataAccessor, $"_{subLoq.Name}Location.Value.Min", $"_{subLoq.Name}Location.Value.Max")}), _package");
                                                if (loqui.SingletonType == SingletonLevel.None)
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
                    if (loqui.SingletonType == SingletonLevel.None)
                    {
                        fg.AppendLine($"public {loqui.Interface(getter: true, internalInterface: true)} {typeGen.Name} => {this.Module.BinaryOverlayClassName(loqui)}.{loqui.TargetObjectGeneration.Name}Factory(new {nameof(BinaryMemoryReadStream)}({dataAccessor}.Slice({currentPosition})), _package, {recConverter});");
                    }
                    else
                    {
                        fg.AppendLine($"private {loqui.Interface(getter: true, internalInterface: true)} _{typeGen.Name} {{ get; private set; }}");
                    }
                }
            }
            else if (loqui.SingletonType == SingletonLevel.None)
            {
                throw new NotImplementedException();
            }
            else
            {
                DataBinaryTranslationGeneration.GenerateWrapperExtraMembers(fg, dataType, objGen, typeGen, currentPosition);
                fg.AppendLine($"private {loqui.Interface(getter: true, internalInterface: true)} _{typeGen.Name} => _{typeGen.Name}_IsSet ? {this.Module.BinaryOverlayClassName(loqui)}.{loqui.TargetObjectGeneration.Name}Factory(new {nameof(BinaryMemoryReadStream)}({DataAccessor(dataAccessor, $"_{typeGen.Name}Location", null)}), _package) : default;");
            }

            if (loqui.SingletonType != SingletonLevel.None)
            {
                fg.AppendLine($"public {loqui.Interface(getter: true, internalInterface: true)} {typeGen.Name} => _{typeGen.Name} ?? new {loqui.DirectTypeName}({(loqui.ThisConstruction ? "this" : null)});");
            }
            else if (typeGen.HasBeenSet && !loqui.TargetObjectGeneration.IsTypelessStruct())
            {
                fg.AppendLine($"public bool {typeGen.Name}_IsSet => _{typeGen.Name}Location.HasValue;");
            }
        }

        public override int? ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
        {
            LoquiType loqui = typeGen as LoquiType;
            if (loqui.TargetObjectGeneration == null) return null;
            var sum = 0;
            foreach (var item in loqui.TargetObjectGeneration.IterateFields(includeBaseClass: true))
            {
                if (!this.Module.TryGetTypeGeneration(item.GetType(), out var gen)) continue;
                sum += gen.ExpectedLength(objGen, item) ?? 0;
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
            string accessor;
            switch (loqui.SingletonType)
            {
                case SingletonLevel.None:
                    accessor = typeGen.Name;
                    break;
                case SingletonLevel.NotNull:
                case SingletonLevel.Singleton:
                    accessor = $"_{typeGen.Name}";
                    break;
                default:
                    throw new NotImplementedException();
            }
            var data = loqui.GetFieldData();
            if (data.MarkerType.HasValue)
            {
                fg.AppendLine($"stream.Position += {packageAccessor}.Meta.SubConstants.HeaderLength; // Skip marker");
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
            return $"{this.Module.BinaryOverlayClass(loqui.TargetObjectGeneration)}.{loqui.TargetObjectGeneration.Name}Factory(new {nameof(BinaryMemoryReadStream)}(s), p)";
        }
    }
}
