using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Generation
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
            TypeGeneration typeGen,
            string writerAccessor,
            string itemAccessor,
            string doMaskAccessor,
            string maskAccessor)
        {
            ByteArrayType stringType = typeGen as ByteArrayType;
            using (var args = new ArgsWrapper(fg,
                $"{this.Namespace}ByteArrayBinaryTranslation.Instance.Write"))
            {
                args.Add($"writer: {writerAccessor}");
                args.Add($"item: {itemAccessor}");
                args.Add($"doMasks: {doMaskAccessor}");
                args.Add($"errorMask: out {maskAccessor}");
                if (stringType.RecordType.HasValue)
                {
                    args.Add($"header: {stringType.RecordType.Value.HeaderName}");
                    args.Add($"lengthLength: {stringType.Length.Value}");
                    args.Add($"nullable: {(stringType.Optional ? "true" : "false")}");
                }
                else
                {
                    args.Add($"length: {stringType.Length.Value}");
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
            ByteArrayType stringType = typeGen as ByteArrayType;
            using (var args = new ArgsWrapper(fg,
                $"var tryGet = {this.Namespace}ByteArrayBinaryTranslation.Instance.Parse"))
            {
                args.Add(nodeAccessor);
                args.Add($"doMasks: {doMaskAccessor}");
                args.Add($"errorMask: out {maskAccessor}");
                if (stringType.RecordType.HasValue)
                {
                    args.Add($"header: {stringType.RecordType.Value.HeaderName}");
                    args.Add($"lengthLength: {stringType.Length.Value}");
                    args.Add($"nullable: {(stringType.Optional ? "true" : "false")}");
                }
                else
                {
                    args.Add($"length: {stringType.Length.Value}");
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
            ByteArrayType stringType = typeGen as ByteArrayType;
            using (var args = new ArgsWrapper(fg,
                $"{retAccessor}{this.Namespace}ByteArrayBinaryTranslation.Instance.Parse",
                (this.Nullable ? string.Empty : $".Bubble((o) => o.Value)")))
            {
                args.Add(nodeAccessor);
                if (CanBeNotNullable)
                {
                    args.Add($"nullable: {Nullable.ToString().ToLower()}");
                }
                args.Add($"doMasks: {doMaskAccessor}");
                args.Add($"errorMask: out {maskAccessor}");
                if (stringType.RecordType.HasValue)
                {
                    args.Add($"header: {stringType.RecordType.Value.HeaderName}");
                    args.Add($"lengthLength: {stringType.Length.Value}");
                    args.Add($"nullable: {(stringType.Optional ? "true" : "false")}");
                }
                else
                {
                    args.Add($"length: {stringType.Length.Value}");
                }
            }
        }
    }
}
