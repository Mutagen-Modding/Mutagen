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

        public override void GenerateCopyIn(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor readerAccessor,
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor)
        {
            ZeroType zero = typeGen as ZeroType;
            fg.AppendLine($"{readerAccessor}.SetPosition({readerAccessor}.Position + {zero.Length});");
        }

        public override void GenerateCopyInRet(
            FileGeneration fg, 
            ObjectGeneration objGen, 
            TypeGeneration targetGen, 
            TypeGeneration typeGen,
            Accessor readerAccessor,
            bool squashedRepeatedList,
            AsyncMode asyncMode,
            Accessor retAccessor,
            Accessor outItemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationAccessor)
        {
            if (asyncMode == AsyncMode.Direct) throw new NotImplementedException();
            ZeroType zero = typeGen as ZeroType;
            fg.AppendLine($"{readerAccessor}.SetPosition({readerAccessor}.Position + {zero.Length});");
        }

        public override void GenerateWrite(
            FileGeneration fg,
            ObjectGeneration objGen, 
            TypeGeneration typeGen,
            Accessor writerAccessor, 
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor)
        {
            ZeroType zero = typeGen as ZeroType;
            fg.AppendLine($"{writerAccessor}.WriteZeros({zero.Length});");
        }

        public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
        {
            throw new NotImplementedException();
        }

        public override int GetPassedAmount(ObjectGeneration objGen, TypeGeneration typeGen) => 0;
    }
}
