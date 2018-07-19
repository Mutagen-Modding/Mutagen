using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class ByteArrayBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<byte[]>
    {
        public ByteArrayBinaryTranslationGeneration()
            : base(nullable: true,
                  typeName: "ByteArray")
        {
        }

        public override void GenerateWrite(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            string writerAccessor,
            Accessor itemAccessor,
            string maskAccessor,
            string translationMaskAccessor)
        {
            var data = typeGen.CustomData[Constants.DATA_KEY] as MutagenFieldData;
            using (var args = new ArgsWrapper(fg,
                $"{this.Namespace}ByteArrayBinaryTranslation.Instance.Write"))
            {
                args.Add($"writer: {writerAccessor}");
                args.Add($"item: {itemAccessor.PropertyOrDirectAccess}");
                args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                args.Add($"errorMask: {maskAccessor}");
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
            string frameAccessor,
            Accessor itemAccessor,
            string maskAccessor,
            string translationMaskAccessor)
        {
            var data = typeGen.CustomData[Constants.DATA_KEY] as MutagenFieldData;
            if (data.HasTrigger)
            {
                fg.AppendLine($"{frameAccessor}.Position += Constants.SUBRECORD_LENGTH;");
            }

            TranslationGeneration.WrapParseCall(
                fg: fg,
                typeGen: typeGen,
                translatorLine: $"{this.Namespace}ByteArrayBinaryTranslation.Instance",
                maskAccessor: maskAccessor,
                itemAccessor: itemAccessor,
                translationMaskAccessor: null,
                indexAccessor: typeGen.IndexEnumInt,
                extraargs: $"frame: {frameAccessor}{(data.HasTrigger ? ".SpawnWithLength(contentLength)" : $".SpawnWithLength({data.Length.Value})")}");
        }

        public override void GenerateCopyInRet(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration targetGen,
            TypeGeneration typeGen,
            string nodeAccessor,
            bool squashedRepeatedList,
            string retAccessor,
            Accessor outItemAccessor,
            string maskAccessor,
            string translationMaskAccessor)
        {
            var data = typeGen.CustomData[Constants.DATA_KEY] as MutagenFieldData;
            using (var args = new ArgsWrapper(fg,
                $"{retAccessor}{this.Namespace}ByteArrayBinaryTranslation.Instance.Parse"))
            {
                args.Add(nodeAccessor);
                args.Add($"errorMask: out {maskAccessor}");
                args.Add($"item: out {outItemAccessor.DirectAccess}");
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
    }
}
