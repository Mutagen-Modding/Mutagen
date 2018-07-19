using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;

namespace Mutagen.Bethesda.Generation
{
    public class ZeroBinaryTranslationGeneration : BinaryTranslationGeneration
    {
        public override bool ShouldGenerateCopyIn(TypeGeneration typeGen)
        {
            return true;
        }

        public override void GenerateCopyIn(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, string readerAccessor, Accessor itemAccessor, string maskAccessor,
            string translationMaskAccessor)
        {
            ZeroType zero = typeGen as ZeroType;
            fg.AppendLine($"{readerAccessor}.SetPosition({readerAccessor}.Position + {zero.Length});");
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
            string maskAccessor,
            string translationMaskAccessor)
        {
            ZeroType zero = typeGen as ZeroType;
            fg.AppendLine($"{readerAccessor}.SetPosition({readerAccessor}.Position + {zero.Length});");
        }

        public override void GenerateWrite(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, string writerAccessor, Accessor itemAccessor, string maskAccessor,
            string translationMaskAccessor)
        {
            ZeroType zero = typeGen as ZeroType;
            fg.AppendLine($"{writerAccessor}.WriteZeros({zero.Length});");
        }

        public override string GetTranslatorInstance(TypeGeneration typeGen)
        {
            throw new NotImplementedException();
        }
    }
}
