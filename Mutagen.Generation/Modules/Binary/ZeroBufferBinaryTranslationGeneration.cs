using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;

namespace Mutagen.Generation
{
    public class ZeroBufferBinaryTranslationGeneration : BinaryTranslationGeneration
    {
        public override void GenerateCopyIn(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            string readerAccessor,
            Accessor itemAccessor,
            string doMaskAccessor,
            string maskAccessor)
        {
            ZeroBufferType zero = typeGen as ZeroBufferType;
            fg.AppendLine($"{readerAccessor}.Position += {zero.Length};");
        }

        public override void GenerateCopyInRet(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            string readerAccessor,
            string retAccessor,
            string doMaskAccessor,
            string maskAccessor)
        {
            ZeroBufferType zero = typeGen as ZeroBufferType;
            fg.AppendLine($"{readerAccessor}.Position += {zero.Length};");
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
            ZeroBufferType zero = typeGen as ZeroBufferType;
            using (var args = new ArgsWrapper(fg,
                $"{this.Namespace}ZeroBufferBinaryTranslation.Instance.Write"))
            {
                args.Add($"writer: {writerAccessor}");
                args.Add($"length: {zero.Length}");
            }
        }
    }
}
