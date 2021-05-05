using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Generation.Modules.Binary;
using Mutagen.Bethesda.Generation.Modules.Plugin;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation.Modules.Pex
{
    public class PrimitivePexTranslationGeneration<T> : BinaryTranslationGeneration
    {
        private int? _ExpectedLength;
        private string typeName;
        protected bool? nullable;
        public bool Nullable => nullable ?? false || typeof(T).GetName().EndsWith("?");
        public bool PreferDirectTranslation = true;
        public delegate bool CustomReadAction(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor reader, Accessor item);
        public CustomReadAction CustomRead;
        public delegate bool CustomWriteAction(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor writer, Accessor item);
        public CustomWriteAction CustomWrite;
        public delegate bool CustomWrapperAction(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor data, Accessor passedLen);
        public CustomWrapperAction CustomWrapper;

        public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
        {
            return $"{Typename(typeGen)}PexTranslation.Instance";
        }

        public PrimitivePexTranslationGeneration(int? expectedLen, string typeName = null, bool? nullable = null)
        {
            this._ExpectedLength = expectedLen;
            this.nullable = nullable;
            this.typeName = typeName ?? typeof(T).GetName().Replace("?", string.Empty);
        }

        protected virtual string ItemWriteAccess(TypeGeneration typeGen, Accessor itemAccessor)
        {
            return $"{itemAccessor}";
        }

        public virtual string Typename(TypeGeneration typeGen) => typeName;

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
            var data = typeGen.CustomData[Constants.DataKey] as MutagenFieldData;
            if (CustomWrite != null)
            {
                if (CustomWrite(fg, objGen, typeGen, writerAccessor, itemAccessor)) return;
            }
            if (data.HasTrigger || !PreferDirectTranslation)
            {
                using (var args = new ArgsWrapper(fg,
                    $"{this.Namespace}{this.Typename(typeGen)}BinaryTranslation.Instance.Write{(typeGen.Nullable ? "Nullable" : null)}"))
                {
                    args.Add($"writer: {writerAccessor}");
                    args.Add($"item: {ItemWriteAccess(typeGen, itemAccessor)}");
                    if (this.DoErrorMasks)
                    {
                        if (typeGen.HasIndex)
                        {
                            args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                        }
                        args.Add($"errorMask: {errorMaskAccessor}");
                    }
                    if (data.RecordType.HasValue
                        && data.HandleTrigger)
                    {
                        args.Add($"header: recordTypeConverter.ConvertToCustom({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
                    }
                    foreach (var writeParam in this.AdditionalWriteParams)
                    {
                        var get = writeParam(
                            objGen: objGen,
                            typeGen: typeGen);
                        if (get.Failed) continue;
                        args.Add(get.Value);
                    }
                }
            }
            else if (typeGen.GetFieldData().Length > 1)
            {
                fg.AppendLine($"{writerAccessor}.Write({itemAccessor}, length: {typeGen.GetFieldData().Length});");
            }
            else
            {
                fg.AppendLine($"{writerAccessor}.Write({itemAccessor});");
            }
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
            var fieldData = typeGen.GetFieldData();
            if (fieldData.HasTrigger)
            {
                fg.AppendLine($"{frameAccessor}.Position += {frameAccessor}.{nameof(MutagenBinaryReadStream.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(RecordHeaderConstants.HeaderLength)};");
            }

            bool hasCustom = false;
            List<string> extraArgs = new List<string>();
            extraArgs.Add($"frame: {frameAccessor}{(fieldData.HasTrigger ? ".SpawnWithLength(contentLength)" : "")}");
            foreach (var writeParam in this.AdditionalCopyInParams)
            {
                var get = writeParam(
                    objGen: objGen,
                    typeGen: typeGen);
                if (get.Failed) continue;
                extraArgs.Add(get.Value);
                hasCustom = true;
            }

            if (CustomRead != null)
            {
                if (CustomRead(fg, objGen, typeGen, frameAccessor, itemAccessor)) return;
            }
            if (PreferDirectTranslation && !hasCustom)
            {
                fg.AppendLine($"{itemAccessor} = {frameAccessor}.Read{typeName}();");
            }
            else
            {
                TranslationGeneration.WrapParseCall(
                    new TranslationWrapParseArgs()
                    {
                        FG = fg,
                        TypeGen = typeGen,
                        TranslatorLine = $"{this.Namespace}{this.Typename(typeGen)}BinaryTranslation.Instance",
                        MaskAccessor = errorMaskAccessor,
                        ItemAccessor = itemAccessor,
                        TranslationMaskAccessor = null,
                        AsyncMode = AsyncMode.Off,
                        IndexAccessor = typeGen.HasIndex ? typeGen.IndexEnumInt : null,
                        ExtraArgs = extraArgs.ToArray(),
                        SkipErrorMask = !this.DoErrorMasks
                    });
            }
        }

        public override void GenerateCopyInRet(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration targetGen,
            TypeGeneration typeGen,
            Accessor nodeAccessor,
            AsyncMode asyncMode,
            Accessor retAccessor,
            Accessor outItemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor,
            Accessor converterAccessor,
            bool inline)
        {
            if (asyncMode != AsyncMode.Off) throw new NotImplementedException();
            if (inline)
            {
                fg.AppendLine($"transl: {this.GetTranslatorInstance(typeGen, getter: false)}.Parse");
            }
            else
            {
                var data = typeGen.GetFieldData();
                if (data.RecordType.HasValue)
                {
                    if (inline)
                    {
                        throw new NotImplementedException();
                    }
                    fg.AppendLine("r.Position += Constants.SUBRECORD_LENGTH;");
                }
                using (var args = new ArgsWrapper(fg,
                    $"{outItemAccessor} = {this.Namespace}{this.Typename(typeGen)}BinaryTranslation.Instance.Parse"))
                {
                    args.Add(nodeAccessor.Access);
                    if (this.DoErrorMasks)
                    {
                        args.Add($"errorMask: {errorMaskAccessor}");
                    }
                    foreach (var writeParam in this.AdditionalCopyInRetParams)
                    {
                        var get = writeParam(
                            objGen: objGen,
                            typeGen: typeGen);
                        if (get.Failed) continue;
                        args.Add(get.Value);
                    }
                }
                fg.AppendLine("return true;");
            }
        }

        public override string GenerateForTypicalWrapper(
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor dataAccessor,
            Accessor packageAccessor)
        {
            return $"BinaryPrimitives.Read{typeGen.TypeName(getter: true)}LittleEndian({dataAccessor})";
        }

        public override async Task GenerateWrapperFields(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor dataAccessor,
            int? currentPosition,
            string passedLengthAccessor,
            DataType dataType = null)
        {
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
            if (data.HasTrigger)
            {
                fg.AppendLine($"private int? _{typeGen.Name}Location;");
            }
            if (data.RecordType.HasValue)
            {
                if (dataType != null) throw new ArgumentException();
                dataAccessor = $"{nameof(HeaderTranslation)}.{nameof(HeaderTranslation.ExtractSubrecordMemory)}({dataAccessor}, _{typeGen.Name}Location.Value, _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)})";
                fg.AppendLine($"public {typeGen.TypeName(getter: true)}{(typeGen.Nullable ? "?" : null)} {typeGen.Name} => _{typeGen.Name}Location.HasValue ? {GenerateForTypicalWrapper(objGen, typeGen, dataAccessor, "_package")} : {typeGen.GetDefault(getter: true)};");
            }
            else
            {
                var expectedLen = await this.ExpectedLength(objGen, typeGen);
                if (this.CustomWrapper != null)
                {
                    if (CustomWrapper(fg, objGen, typeGen, dataAccessor, passedLengthAccessor)) return;
                }
                if (dataType == null)
                {
                    if (typeGen.Nullable)
                    {
                        if (!typeGen.CanBeNullable(getter: true))
                        {
                            throw new NotImplementedException();
                            //fg.AppendLine($"public bool {typeGen.Name}_IsSet => {dataAccessor}.Length >= {(currentPosition + this.ExpectedLength(objGen, typeGen).Value)};");
                            //fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => {GenerateForTypicalWrapper(objGen, typeGen, $"{dataAccessor}.Span.Slice({currentPosition}, {this.ExpectedLength(objGen, typeGen).Value})", "_package")};");
                        }
                        else
                        {
                            string passed = int.TryParse(passedLengthAccessor.TrimStart("0x"), System.Globalization.NumberStyles.HexNumber, null, out var passedInt) ? (passedInt + expectedLen.Value).ToString() : $"({passedLengthAccessor} + {expectedLen.Value})";
                            fg.AppendLine($"public {typeGen.TypeName(getter: true)}? {typeGen.Name} => {dataAccessor}.Length >= {passed} ? {GenerateForTypicalWrapper(objGen, typeGen, $"{dataAccessor}.Slice({passedLengthAccessor}{(expectedLen != null ? $", 0x{expectedLen.Value:X}" : null)})", "_package")} : {typeGen.GetDefault(getter: true)};");
                        }
                    }
                    else
                    {
                        fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => {GenerateForTypicalWrapper(objGen, typeGen, $"{dataAccessor}.Slice({passedLengthAccessor ?? "0x0"}{(expectedLen != null ? $", 0x{expectedLen.Value:X}" : null)})", "_package")};");
                    }
                }
                else
                {
                    DataBinaryTranslationGeneration.GenerateWrapperExtraMembers(fg, dataType, objGen, typeGen, passedLengthAccessor);
                    fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => _{typeGen.Name}_IsSet ? {GenerateForTypicalWrapper(objGen, typeGen, $"{dataAccessor}.Slice(_{typeGen.Name}Location{(expectedLen.HasValue ? $", {expectedLen.Value}" : null)})", "_package")} : {typeGen.GetDefault(getter: true)};");
                }
            }
        }

        public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
        {
            return this._ExpectedLength;
        }
    }
}
