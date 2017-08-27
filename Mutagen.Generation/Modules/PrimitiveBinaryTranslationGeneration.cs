using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Generation
{
    public class PrimitiveBinaryTranslationGeneration<T> : BinaryTranslationGeneration
    {
        private string typeName;
        private bool? nullable;
        public bool Nullable => nullable ?? false || typeof(T).GetName().EndsWith("?");
        public bool CanBeNotNullable = true;

        public PrimitiveBinaryTranslationGeneration(string typeName = null, bool? nullable = null)
        {
            this.nullable = nullable;
            this.typeName = typeName ?? typeof(T).GetName().Replace("?", string.Empty);
        }

        public override void GenerateWrite(
            FileGeneration fg,
            TypeGeneration typeGen,
            string writerAccessor, 
            string itemAccessor,
            string doMaskAccessor,
            string maskAccessor)
        {
            using (var args = new ArgsWrapper(fg,
                $"{this.Namespace}{this.typeName}BinaryTranslation.Instance.Write"))
            {
                args.Add($"writer: {writerAccessor}");
                args.Add($"item: {itemAccessor}");
                args.Add($"doMasks: {doMaskAccessor}");
                args.Add($"errorMask: out {maskAccessor}");
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
            using (var args = new ArgsWrapper(fg,
                $"var tryGet = {this.Namespace}{this.typeName}BinaryTranslation.Instance.Parse"))
            {
                args.Add(nodeAccessor);
                args.Add($"doMasks: {doMaskAccessor}");
                args.Add($"errorMask: out {maskAccessor}");
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
            using (var args = new ArgsWrapper(fg,
                $"{retAccessor}{this.Namespace}{this.typeName}BinaryTranslation.Instance.Parse",
                (this.Nullable ? string.Empty : $".Bubble((o) => o.Value)")))
            {
                args.Add(nodeAccessor);
                if (CanBeNotNullable)
                {
                    args.Add($"nullable: {Nullable.ToString().ToLower()}");
                }
                args.Add($"doMasks: {doMaskAccessor}");
                args.Add($"errorMask: out {maskAccessor}");
            }
        }
    }
}
