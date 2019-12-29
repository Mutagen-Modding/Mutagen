using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Binary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noggog;

namespace Mutagen.Bethesda.Generation
{
    public class ByteArrayBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<byte[]>
    {
        public ByteArrayBinaryTranslationGeneration()
            : base(nullable: true,
                  expectedLen: null,
                  typeName: "ByteArray")
        {
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
            var data = typeGen.CustomData[Constants.DataKey] as MutagenFieldData;
            using (var args = new ArgsWrapper(fg,
                $"{this.Namespace}ByteArrayBinaryTranslation.Instance.Write"))
            {
                args.Add($"writer: {writerAccessor}");
                args.Add($"item: {itemAccessor.DirectAccess}");
                if (this.DoErrorMasks)
                {
                    args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                    args.Add($"errorMask: {errorMaskAccessor}");
                }
                if (data.RecordType.HasValue)
                {
                    args.Add($"header: recordTypeConverter.ConvertToCustom({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
                    args.Add($"nullable: {(data.Optional ? "true" : "false")}");
                }
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

            TranslationGeneration.WrapParseCall(
                new TranslationWrapParseArgs()
                {
                    FG = fg,
                    TypeGen = typeGen,
                    TranslatorLine = $"{this.Namespace}ByteArrayBinaryTranslation.Instance",
                    MaskAccessor = errorMaskAccessor,
                    ItemAccessor = itemAccessor,
                    IndexAccessor = typeGen.IndexEnumInt,
                    ExtraArgs = $"frame: {frameAccessor}{(data.HasTrigger ? ".SpawnWithLength(contentLength)" : $".SpawnWithLength({data.Length.Value})")}".Single(),
                    SkipErrorMask = !this.DoErrorMasks
                });
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
            Accessor translationAccessor)
        {
            var data = typeGen.CustomData[Constants.DataKey] as MutagenFieldData;
            using (var args = new ArgsWrapper(fg,
                $"{retAccessor}{Loqui.Generation.Utility.Await(asyncMode)}{this.Namespace}ByteArrayBinaryTranslation.Instance.Parse",
                suffixLine: Loqui.Generation.Utility.ConfigAwait(asyncMode)))
            {
                args.Add(nodeAccessor.DirectAccess);
                if (this.DoErrorMasks)
                {
                    args.Add($"errorMask: out {errorMaskAccessor}");
                }
                if (asyncMode == AsyncMode.Off)
                {
                    args.Add($"item: out {outItemAccessor.DirectAccess}");
                }
                if (data.HasTrigger)
                {
                    args.Add($"length: subLength");
                }
                else
                {
                    args.Add($"length: {data.Length.Value}");
                }
            }
        }

        public override void GenerateWrapperFields(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor dataAccessor, 
            int currentPosition,
            DataType dataType = null)
        {
            var data = typeGen.CustomData[Constants.DataKey] as MutagenFieldData;
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
                fg.AppendLine($"public bool {typeGen.Name}_IsSet => _{typeGen.Name}Location.HasValue;");
            }
            if (data.RecordType.HasValue)
            {
                if (dataType != null)
                {
                    throw new ArgumentException();
                }
                fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => _{typeGen.Name}Location.HasValue ? {nameof(HeaderTranslation)}.{nameof(HeaderTranslation.ExtractSubrecordSpan)}(_data, _{typeGen.Name}Location.Value, _package.Meta).ToArray() : default;");
            }
            else
            {
                if (dataType == null)
                {
                    if (typeGen.HasBeenSet)
                    {
                        fg.AppendLine($"public bool {typeGen.HasBeenSetAccessor()} => {dataAccessor}.Length >= {(currentPosition + this.ExpectedLength(objGen, typeGen).Value)};");
                    }
                    fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => {dataAccessor}.Span.Slice({currentPosition}, {data.Length.Value}).ToArray();");
                }
                else
                {
                    if (typeGen.HasBeenSet)
                    {
                        fg.AppendLine($"public bool {typeGen.HasBeenSetAccessor()} => {dataAccessor}.Length >= _{typeGen.Name}Location + {this.ExpectedLength(objGen, typeGen).Value};");
                    }
                    DataBinaryTranslationGeneration.GenerateWrapperExtraMembers(fg, dataType, objGen, typeGen, currentPosition);
                    fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => _{typeGen.Name}_IsSet ? {dataAccessor}.Span.Slice(_{typeGen.Name}Location, {this.ExpectedLength(objGen, typeGen).Value}).ToArray() : default;");
                }
            }
        }

        public override int GetPassedAmount(ObjectGeneration objGen, TypeGeneration typeGen)
        {
            var data = typeGen.CustomData[Constants.DataKey] as MutagenFieldData;
            if (!data.RecordType.HasValue)
            {
                return checked((int)data.Length.Value);
            }
            else
            {
                return 0;
            }
        }

        public override int? ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
        {
            ByteArrayType bType = typeGen as ByteArrayType;
            return bType.Length;
        }
    }
}
