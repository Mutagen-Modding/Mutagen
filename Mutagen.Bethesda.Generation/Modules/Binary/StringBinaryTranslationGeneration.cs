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
    public class StringBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<string>
    {
        public override bool AllowDirectParse(
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            bool squashedRepeatedList)
        {
            var str = typeGen as Mutagen.Bethesda.Generation.StringType;
            if (str.BinaryType != StringBinaryType.NullTerminate) return false;
            return !squashedRepeatedList;
        }

        public override bool AllowDirectWrite(ObjectGeneration objGen, TypeGeneration typeGen)
        {
            var str = typeGen as Mutagen.Bethesda.Generation.StringType;
            if (str.BinaryType != StringBinaryType.NullTerminate) return false;
            return true;
        }

        public StringBinaryTranslationGeneration()
            : base(nullable: true, expectedLen: null)
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
            Mutagen.Bethesda.Generation.StringType stringType = typeGen as Mutagen.Bethesda.Generation.StringType;
            var data = typeGen.CustomData[Constants.DataKey] as MutagenFieldData;
            using (var args = new ArgsWrapper(fg,
                $"{this.Namespace}StringBinaryTranslation.Instance.Write{(typeGen.HasBeenSet ? "Nullable" : null)}"))
            {
                args.Add($"writer: {writerAccessor}");
                args.Add($"item: {itemAccessor.DirectAccess}");
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
                }
                else if (data.Length.HasValue)
                {
                    args.Add($"length: {data.Length.Value}"); 
                }
                args.Add($"binaryType: {nameof(StringBinaryType)}.{stringType.BinaryType}");
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
                fg.AppendLine($"{frameAccessor}.Position += {frameAccessor}.{nameof(MutagenBinaryReadStream.MetaData)}.{nameof(GameConstants.SubConstants)}.{nameof(RecordHeaderConstants.HeaderLength)};");
            }

            List<string> extraArgs = new List<string>();
            extraArgs.Add($"frame: {frameAccessor}{(data.HasTrigger ? ".SpawnWithLength(contentLength)" : null)}");
            extraArgs.Add($"parseWhole: true");

            TranslationGeneration.WrapParseCall(
                new TranslationWrapParseArgs()
                {
                    FG = fg,
                    TypeGen = typeGen,
                    TranslatorLine = $"{this.Namespace}StringBinaryTranslation.Instance",
                    MaskAccessor = errorMaskAccessor,
                    ItemAccessor = itemAccessor,
                    TranslationMaskAccessor = null,
                    IndexAccessor = typeGen.HasIndex ? typeGen.IndexEnumInt : null,
                    ExtraArgs = extraArgs.ToArray(),
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
            Accessor translationMaskAccessor)
        {
            if (asyncMode != AsyncMode.Off) throw new NotImplementedException();
            var data = typeGen.GetFieldData();
            using (var args = new ArgsWrapper(fg,
                $"{retAccessor}{this.Namespace}StringBinaryTranslation.Instance.Parse"))
            {
                args.Add(nodeAccessor.DirectAccess);
                if (this.DoErrorMasks)
                {
                    args.Add($"errorMask: {errorMaskAccessor}");
                }
                args.Add($"item: out {outItemAccessor.DirectAccess}");
                args.Add($"parseWhole: {(data.HasTrigger ? "true" : "false")}");

                if (data.Length.HasValue)
                {
                    args.Add($"length: {data.Length.Value}");
                }
            }
        }

        public override string GenerateForTypicalWrapper(
            ObjectGeneration objGen, 
            TypeGeneration typeGen, 
            Accessor dataAccessor,
            Accessor packageAccessor)
        {
            if (typeGen.GetFieldData().HasTrigger)
            {
                return $"{nameof(BinaryStringUtility)}.{nameof(BinaryStringUtility.ProcessWholeToZString)}({dataAccessor})";
            }
            else
            {
                return $"{nameof(BinaryStringUtility)}.{nameof(BinaryStringUtility.ParseUnknownLengthString)}({dataAccessor})";
            }
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
            if (data.BinaryOverlayFallback != BinaryGenerationType.Normal
                || data.RecordType.HasValue
                || this.ExpectedLength(objGen, typeGen) != null)
            {
                base.GenerateWrapperFields(fg, objGen, typeGen, dataAccessor, currentPosition, dataType);
            }
        }
    }
}
