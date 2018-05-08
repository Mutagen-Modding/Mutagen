using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class StringBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<StringType>
    {
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
            string doMaskAccessor,
            string maskAccessor)
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
                    args.Add($"errorMask: {maskAccessor}");
                }
                else
                {
                    args.Add($"doMasks: {doMaskAccessor}");
                    args.Add($"errorMask: out {maskAccessor}");
                }
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
            string nodeAccessor,
            Accessor itemAccessor,
            string doMaskAccessor,
            string maskAccessor)
        {
            var data = typeGen.CustomData[Constants.DATA_KEY] as MutagenFieldData;
            if (data.HasTrigger)
            {
                fg.AppendLine($"{nodeAccessor}.Position += Constants.SUBRECORD_LENGTH;");
            }
            using (var args = new ArgsWrapper(fg,
                $"var {typeGen.Name}tryGet = {this.Namespace}StringBinaryTranslation.Instance.Parse"))
            {
                if (data.HasTrigger)
                {
                    args.Add($"frame: {nodeAccessor}.SpawnWithLength(contentLength)");
                }
                else
                {
                    args.Add($"frame: {nodeAccessor}");
                }
                args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                args.Add($"parseWhole: true");
                args.Add($"errorMask: {maskAccessor}");
            }
            if (itemAccessor.PropertyAccess != null)
            {
                fg.AppendLine($"{itemAccessor.PropertyAccess}.{nameof(INotifyingCollectionExt.SetIfSucceeded)}({typeGen.Name}tryGet);");
            }
            else
            {
                fg.AppendLine("if ({typeGen.Name}tryGet.Succeeded)");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"{itemAccessor.DirectAccess} = {typeGen.Name}tryGet.Value;");
                }
            }
        }

        public override void GenerateCopyInRet(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration targetGen,
            TypeGeneration typeGen,
            string nodeAccessor,
            bool squashedRepeatedList,
            Accessor retAccessor,
            string doMaskAccessor,
            string maskAccessor)
        {
            var data = typeGen.CustomData[Constants.DATA_KEY] as MutagenFieldData;
            using (var args = new ArgsWrapper(fg,
                $"{retAccessor.DirectAccess}{this.Namespace}StringBinaryTranslation.Instance.Parse",
                (this.Nullable ? string.Empty : $".Bubble((o) => o.Value)")))
            {
                args.Add(nodeAccessor);
                args.Add($"doMasks: {doMaskAccessor}");
                args.Add($"errorMask: out {maskAccessor}");
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
