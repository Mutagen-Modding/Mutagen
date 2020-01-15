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
    public class PrimitiveBinaryTranslationGeneration<T> : BinaryTranslationGeneration
    {
        private int? _ExpectedLength;
        private string typeName;
        protected bool? nullable;
        public bool Nullable => nullable ?? false || typeof(T).GetName().EndsWith("?");
        public bool PreferDirectTranslation = true;
        public Action<FileGeneration, Accessor, Accessor> CustomRead;
        public Action<FileGeneration, Accessor, Accessor> CustomWrite;

        public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
        {
            return $"{Typename(typeGen)}BinaryTranslation.Instance";
        }

        public PrimitiveBinaryTranslationGeneration(int? expectedLen, string typeName = null, bool? nullable = null)
        {
            this._ExpectedLength = expectedLen;
            this.nullable = nullable;
            this.typeName = typeName ?? typeof(T).GetName().Replace("?", string.Empty);
        }

        protected virtual string ItemWriteAccess(Accessor itemAccessor)
        {
            return itemAccessor.DirectAccess;
        }

        public virtual string Typename(TypeGeneration typeGen) => typeName;

        public override void GenerateWrite(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor writerAccessor,
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor)
        {
            var data = typeGen.CustomData[Constants.DataKey] as MutagenFieldData;
            if (CustomWrite != null)
            {
                CustomWrite(fg, writerAccessor, itemAccessor);
            }
            else if (data.HasTrigger || !PreferDirectTranslation)
            {
                using (var args = new ArgsWrapper(fg,
                    $"{this.Namespace}{this.Typename(typeGen)}BinaryTranslation.Instance.Write"))
                {
                    args.Add($"writer: {writerAccessor}");
                    args.Add($"item: {ItemWriteAccess(itemAccessor)}");
                    if (this.DoErrorMasks)
                    {
                        if (typeGen.HasIndex)
                        {
                            args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                        }
                        args.Add($"errorMask: {errorMaskAccessor}");
                    }
                    if (data.RecordType.HasValue)
                    {
                        args.Add($"header: recordTypeConverter.ConvertToCustom({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
                        args.Add($"nullable: {(data.Optional ? "true" : "false")}");
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
            else
            {
                fg.AppendLine($"{writerAccessor.DirectAccess}.Write({itemAccessor.DirectAccess});");
            }
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
            var data = typeGen.CustomData[Constants.DataKey] as MutagenFieldData;
            if (data.HasTrigger)
            {
                fg.AppendLine($"{frameAccessor}.Position += {frameAccessor}.{nameof(MutagenBinaryReadStream.MetaData)}.{nameof(MetaDataConstants.SubConstants)}.{nameof(RecordConstants.HeaderLength)};");
            }


            List<string> extraArgs = new List<string>();
            extraArgs.Add($"frame: {frameAccessor}{(data.HasTrigger ? ".SpawnWithLength(contentLength)" : "")}");
            foreach (var writeParam in this.AdditionalCopyInParams)
            {
                var get = writeParam(
                    objGen: objGen,
                    typeGen: typeGen);
                if (get.Failed) continue;
                extraArgs.Add(get.Value);
            }

            if (CustomRead != null)
            {
                CustomRead(fg, frameAccessor, itemAccessor);
            }
            else if (PreferDirectTranslation)
            {
                fg.AppendLine($"{itemAccessor.DirectAccess} = {frameAccessor.DirectAccess}.Read{typeName}();");
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
            Accessor translationMaskAccessor)
        {
            if (asyncMode != AsyncMode.Off) throw new NotImplementedException();
            if (typeGen.TryGetFieldData(out var data)
                && data.RecordType.HasValue)
            {
                fg.AppendLine("r.Position += Constants.SUBRECORD_LENGTH;");
            }
            using (var args = new ArgsWrapper(fg,
                $"{retAccessor}{this.Namespace}{this.Typename(typeGen)}BinaryTranslation.Instance.Parse"))
            {
                args.Add(nodeAccessor.DirectAccess);
                if (this.DoErrorMasks)
                {
                    args.Add($"errorMask: {errorMaskAccessor}");
                }
                args.Add($"translationMask: {translationMaskAccessor}");
                foreach (var writeParam in this.AdditionalCopyInRetParams)
                {
                    var get = writeParam(
                        objGen: objGen,
                        typeGen: typeGen);
                    if (get.Failed) continue;
                    args.Add(get.Value);
                }
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

        public override void GenerateWrapperFields(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor dataAccessor,
            int? currentPosition,
            DataType dataType = null)
        {
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
            if (data.HasTrigger)
            {
                fg.AppendLine($"private int? _{typeGen.Name}Location;");
                if (typeGen.HasBeenSet)
                {
                    fg.AppendLine($"public bool {typeGen.Name}_IsSet => _{typeGen.Name}Location.HasValue;");
                }
            }
            if (data.RecordType.HasValue)
            {
                if (dataType != null)
                {
                    throw new ArgumentException();
                }
                dataAccessor = $"{nameof(HeaderTranslation)}.{nameof(HeaderTranslation.ExtractSubrecordSpan)}({dataAccessor}, _{typeGen.Name}Location.Value, _package.Meta)";
                fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => _{typeGen.Name}Location.HasValue ? {GenerateForTypicalWrapper(objGen, typeGen, dataAccessor, "_package")} : default;");
            }
            else
            {
                if (this.ExpectedLength(objGen, typeGen) == null)
                {
                    throw new NotImplementedException();
                }
                if (dataType == null)
                {
                    if (typeGen.HasBeenSet)
                    {
                        fg.AppendLine($"public bool {typeGen.HasBeenSetAccessor()} => {dataAccessor}.Length >= {(currentPosition + this.ExpectedLength(objGen, typeGen).Value)};");
                    }
                    fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => {GenerateForTypicalWrapper(objGen, typeGen, $"{dataAccessor}.Span.Slice({currentPosition}, {this.ExpectedLength(objGen, typeGen).Value})", "_package")};");
                }
                else
                {
                    if (typeGen.HasBeenSet)
                    {
                        fg.AppendLine($"public bool {typeGen.HasBeenSetAccessor()} => {dataAccessor}.Length >= _{typeGen.Name}Location + {this.ExpectedLength(objGen, typeGen).Value};");
                    }
                    DataBinaryTranslationGeneration.GenerateWrapperExtraMembers(fg, dataType, objGen, typeGen, currentPosition);
                    fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => _{typeGen.Name}_IsSet ? {GenerateForTypicalWrapper(objGen, typeGen, $"{dataAccessor}.Span.Slice(_{typeGen.Name}Location, {this.ExpectedLength(objGen, typeGen).Value})", "_package")} : default;");
                }
            }
        }

        public override int? ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
        {
            return typeGen.GetFieldData().Length ?? this._ExpectedLength;
        }
    }
}
