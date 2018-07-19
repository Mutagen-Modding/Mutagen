using Loqui;
using Loqui.Generation;
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
            return !squashedRepeatedList;
        }

        public StringBinaryTranslationGeneration()
            : base(nullable: true)
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
            Mutagen.Bethesda.Generation.StringType stringType = typeGen as Mutagen.Bethesda.Generation.StringType;
            var data = typeGen.CustomData[Constants.DATA_KEY] as MutagenFieldData;
            using (var args = new ArgsWrapper(fg,
                $"{this.Namespace}StringBinaryTranslation.Instance.Write"))
            {
                args.Add($"writer: {writerAccessor}");
                args.Add($"item: {itemAccessor.PropertyOrDirectAccess}");
                if (typeGen.HasIndex)
                {
                    args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                }
                args.Add($"errorMask: {maskAccessor}");
                if (data.RecordType.HasValue)
                {
                    args.Add($"header: recordTypeConverter.ConvertToCustom({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
                    args.Add($"nullable: {(data.Optional ? "true" : "false")}");
                }
                else if (data.Length.HasValue)
                {
                    args.Add($"length: {data.Length.Value}");
                }
                if (!stringType.NullTerminate)
                {
                    args.Add($"nullTerminate: false");
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

            List<string> extraArgs = new List<string>();
            extraArgs.Add($"frame: {frameAccessor}{(data.HasTrigger ? ".SpawnWithLength(contentLength)" : null)}");
            extraArgs.Add($"parseWhole: true");

            TranslationGeneration.WrapParseCall(
                fg: fg,
                typeGen: typeGen,
                translatorLine: $"{this.Namespace}StringBinaryTranslation.Instance",
                maskAccessor: maskAccessor,
                itemAccessor: itemAccessor,
                translationMaskAccessor: null,
                indexAccessor: typeGen.HasIndex ? typeGen.IndexEnumInt : null,
                extraargs: extraArgs.ToArray());
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
                $"{retAccessor}{this.Namespace}StringBinaryTranslation.Instance.Parse"))
            {
                args.Add(nodeAccessor);
                args.Add($"errorMask: {maskAccessor}");
                args.Add($"item: out {outItemAccessor.DirectAccess}");
                args.Add($"parseWhole: {(squashedRepeatedList ? "false" : "true")}");

                if (data.RecordType.HasValue)
                {
                    args.Add($"header: recordTypeConverter.Convert({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
                }
                else if (data.Length.HasValue)
                {
                    args.Add($"length: {data.Length.Value}");
                }
            }
        }
    }
}
