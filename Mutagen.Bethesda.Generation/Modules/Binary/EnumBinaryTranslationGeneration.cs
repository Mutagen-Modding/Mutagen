using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class EnumBinaryTranslationGeneration : BinaryTranslationGeneration
    {
        public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
        {
            var eType = typeGen as EnumType;
            return $"EnumBinaryTranslation<{eType.NoNullTypeName}>.Instance";
        }

        public override bool AllowDirectWrite(
            ObjectGeneration objGen,
            TypeGeneration typeGen) => false;
        public override bool AllowDirectParse(
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            bool squashedRepeatedList) => false;

        public override void GenerateWrite(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor writerAccessor,
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor)
        {
            var eType = typeGen as EnumType;
            var data = typeGen.CustomData[Constants.DATA_KEY] as MutagenFieldData;
            using (var args = new ArgsWrapper(fg,
                $"{Namespace}EnumBinaryTranslation<{eType.NoNullTypeName}>.Instance.Write"))
            {
                args.Add(writerAccessor.DirectAccess);
                args.Add($"{itemAccessor.DirectAccess}");
                args.Add($"length: {eType.ByteLength}");
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
            var data = typeGen.CustomData[Constants.DATA_KEY] as MutagenFieldData;
            var eType = typeGen as EnumType;
            if (data.HasTrigger)
            {
                fg.AppendLine($"{frameAccessor}.Position += Mutagen.Bethesda.Constants.SUBRECORD_LENGTH;");
            }

            TranslationGeneration.WrapParseCall(
                new TranslationWrapParseArgs()
                {
                    FG = fg,
                    TypeGen = typeGen,
                    TranslatorLine = $"EnumBinaryTranslation<{eType.NoNullTypeName}>.Instance",
                    MaskAccessor = errorMaskAccessor,
                    ItemAccessor = itemAccessor,
                    TranslationMaskAccessor = null,
                    IndexAccessor = typeGen.IndexEnumInt,
                    ExtraArgs = $"frame: {frameAccessor}{(data.HasTrigger ? ".SpawnWithLength(contentLength)" : $".SpawnWithLength({eType.ByteLength})")}".Single(),
                    SkipErrorMask = !this.DoErrorMasks
                });
        }

        public override void GenerateCopyInRet(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration targetGen,
            TypeGeneration typeGen,
            Accessor nodeAccessor,
            bool squashedRepeatedList,
            AsyncMode asyncMode,
            Accessor retAccessor,
            Accessor outItemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationAccessor)
        {
            var eType = typeGen as EnumType;
            using (var args = new ArgsWrapper(fg,
                $"{retAccessor}{this.Namespace}EnumBinaryTranslation<{eType.NoNullTypeName}>.Instance.Parse"))
            {
                args.Add($"frame: {nodeAccessor}.SpawnWithLength({eType.ByteLength})");
                if (asyncMode == AsyncMode.Off)
                {
                    args.Add($"item: out {outItemAccessor.DirectAccess}");
                }
                if (this.DoErrorMasks)
                {
                    args.Add($"errorMask: {errorMaskAccessor}");
                }
            }
        }

        public override void GenerateWrapperFields(
            FileGeneration fg,
            ObjectGeneration objGen, 
            TypeGeneration typeGen, 
            Accessor dataAccessor,
            int currentPosition,
            DataType dataType)
        {
            var eType = typeGen as EnumType;
            var data = typeGen.CustomData[Constants.DATA_KEY] as MutagenFieldData;

            var posStr = dataType == null ? $"{currentPosition}" : $"_{typeGen.Name}Location";
            var slice = $"{dataAccessor}.Span.Slice({posStr}, {eType.ByteLength})";

            string getType;
            switch (eType.ByteLength)
            {
                case 1:
                    getType = $"{slice}[{posStr}]";
                    break;
                case 2:
                    getType = $"BinaryPrimitives.ReadUInt16LittleEndian({slice})";
                    break;
                case 4:
                    getType = $"BinaryPrimitives.ReadInt32LittleEndian({slice})";
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (data.RecordType.HasValue)
            {
                throw new NotImplementedException();
            }
            else
            {
                if (dataType == null)
                {
                    fg.AppendLine($"public {eType.TypeName(getter: true)} {eType.Name} => ({eType.TypeName(getter: true)}){getType};");
                }
                else
                {
                    DataBinaryTranslationGeneration.GenerateWrapperExtraMembers(fg, dataType, objGen, typeGen, currentPosition);
                    fg.AppendLine($"public {eType.TypeName(getter: true)} {eType.Name} => _{typeGen.Name}_IsSet ? ({eType.TypeName(getter: true)}){getType} : default;");
                }
            }

        }

        public override int GetPassedAmount(ObjectGeneration objGen, TypeGeneration typeGen)
        {
            var eType = typeGen as EnumType;
            return eType.ByteLength;
        }
    }
}
