using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Generation
{
    public class StringBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<StringType>
    {
        public StringBinaryTranslationGeneration()
            : base(nullable: true)
        {
            CanBeNotNullable = false;
        }

        public override void GenerateWrite(
            FileGeneration fg,
            TypeGeneration typeGen,
            string writerAccessor,
            string itemAccessor,
            string doMaskAccessor,
            string maskAccessor)
        {
            var data = typeGen.CustomData[MutagenFieldData.DATA_KEY] as MutagenFieldData;
            using (var args = new ArgsWrapper(fg,
                $"{this.Namespace}StringBinaryTranslation.Instance.Write"))
            {
                args.Add($"writer: {writerAccessor}");
                args.Add($"item: {itemAccessor}");
                args.Add($"doMasks: {doMaskAccessor}");
                args.Add($"errorMask: out {maskAccessor}");
                if (data.RecordType.HasValue)
                {
                    args.Add($"header: {data.RecordType.Value.HeaderName}");
                }
                else
                {
                    args.Add($"length: {data.Length.Value}");
                }
            }
        }

        public override void GenerateCopyIn(
            FileGeneration fg,
            TypeGeneration typeGen,
            string nodeAccessor,
            string itemAccessor,
            string doMaskAccessor,
            string maskAccessor)
        {
            var data = typeGen.CustomData[MutagenFieldData.DATA_KEY] as MutagenFieldData;
            using (var args = new ArgsWrapper(fg,
                $"var tryGet = {this.Namespace}StringBinaryTranslation.Instance.Parse"))
            {
                args.Add(nodeAccessor);
                args.Add($"doMasks: {doMaskAccessor}");
                args.Add($"errorMask: out {maskAccessor}");
                if (data.RecordType.HasValue)
                {
                    args.Add($"header: {data.RecordType.Value.HeaderName}");
                }
                else
                {
                    args.Add($"length: {data.Length.Value}");
                }
            }
            fg.AppendLine("if (tryGet.Succeeded)");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"{itemAccessor} = tryGet.Value;");
            }
        }

        public override void GenerateCopyInRet(
            FileGeneration fg,
            TypeGeneration typeGen,
            string nodeAccessor,
            string retAccessor,
            string doMaskAccessor,
            string maskAccessor)
        {
            var data = typeGen.CustomData[MutagenFieldData.DATA_KEY] as MutagenFieldData;
            using (var args = new ArgsWrapper(fg,
                $"{retAccessor}{this.Namespace}StringBinaryTranslation.Instance.Parse",
                (this.Nullable ? string.Empty : $".Bubble((o) => o.Value)")))
            {
                args.Add(nodeAccessor);
                if (CanBeNotNullable)
                {
                    args.Add($"nullable: {Nullable.ToString().ToLower()}");
                }
                args.Add($"doMasks: {doMaskAccessor}");
                args.Add($"errorMask: out {maskAccessor}");
                if (data.RecordType.HasValue)
                {
                    args.Add($"header: {data.RecordType.Value.HeaderName}");
                }
                else
                {
                    args.Add($"length: {data.Length.Value}");
                }
            }
        }
    }
}
