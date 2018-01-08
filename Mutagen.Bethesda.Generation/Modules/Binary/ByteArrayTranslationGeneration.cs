using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class ByteArrayTranslationGeneration : PrimitiveBinaryTranslationGeneration<byte[]>
    {
        public ByteArrayTranslationGeneration()
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
            string doMaskAccessor,
            string maskAccessor)
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
                $"var {typeGen.Name}tryGet = {this.Namespace}ByteArrayBinaryTranslation.Instance.Parse"))
            {
                if (data.HasTrigger)
                {
                    args.Add($"{nodeAccessor}.Spawn(contentLength)");
                }
                else
                {
                    args.Add($"frame: {nodeAccessor}.Spawn(new ContentLength({data.Length.Value}))");
                }
                args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                args.Add($"errorMask: {maskAccessor}");
            }
            if (itemAccessor.PropertyAccess != null)
            {
                fg.AppendLine($"{itemAccessor.PropertyAccess}.{nameof(INotifyingCollectionExt.SetIfSucceeded)}({typeGen.Name}tryGet);");
            }
            else
            {
                fg.AppendLine($"if ({typeGen.Name}tryGet.Succeeded)");
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
            Accessor retAccessor,
            string doMaskAccessor,
            string maskAccessor)
        {
            var data = typeGen.CustomData[Constants.DATA_KEY] as MutagenFieldData;
            using (var args = new ArgsWrapper(fg,
                $"{retAccessor.DirectAccess}{this.Namespace}ByteArrayBinaryTranslation.Instance.Parse",
                (this.Nullable ? string.Empty : $".Bubble((o) => o.Value)")))
            {
                args.Add(nodeAccessor);
                args.Add($"errorMask: out {maskAccessor}");
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
