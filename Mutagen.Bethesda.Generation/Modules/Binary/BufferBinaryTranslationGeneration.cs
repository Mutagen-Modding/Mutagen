using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;

namespace Mutagen.Bethesda.Generation
{
    public class BufferBinaryTranslationGeneration : BinaryTranslationGeneration
    {
        public override string GetTranslatorInstance(TypeGeneration typeGen)
        {
            return $"ByteArrayBinaryTranslation.Instance";
        }

        public override void GenerateCopyIn(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            string readerAccessor,
            Accessor itemAccessor,
            string maskAccessor)
        {
            BufferType zero = typeGen as BufferType;
            fg.AppendLine($"{readerAccessor}.Position += {zero.Length};");
        }

        public override void GenerateCopyInRet(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration targetGen,
            TypeGeneration typeGen,
            string readerAccessor,
            bool squashedRepeatedList,
            string retAccessor,
            Accessor outItemAccessor,
            string maskAccessor)
        {
            if (squashedRepeatedList)
            {
                throw new NotImplementedException();
            }
            BufferType zero = typeGen as BufferType;
            fg.AppendLine($"{readerAccessor}.Position += {zero.Length};");
        }

        public override void GenerateWrite(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            string writerAccessor,
            Accessor itemAccessor,
            string maskAccessor)
        {
            BufferType zero = typeGen as BufferType;
            using (var args = new ArgsWrapper(fg,
                $"{this.Namespace}ByteArrayBinaryTranslation.Instance.Write"))
            {
                args.Add($"writer: {writerAccessor}");
                if (zero.Static)
                {
                    args.Add($"item: {objGen.ExtCommonName}.{typeGen.Name}");
                }
                else
                {
                    args.Add($"item: {itemAccessor.PropertyOrDirectAccess}");
                }
            }
        }
    }
}
