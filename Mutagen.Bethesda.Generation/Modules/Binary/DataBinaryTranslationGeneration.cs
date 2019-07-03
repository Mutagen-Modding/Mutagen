using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;

namespace Mutagen.Bethesda.Generation
{
    public class DataBinaryTranslationGeneration : BinaryTranslationGeneration
    {
        public override bool ShouldGenerateCopyIn(TypeGeneration typeGen) => true;
        public override bool ShouldGenerateWrite(TypeGeneration typeGen) => true;

        public override void GenerateCopyIn(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor readerAccessor, 
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor)
        {
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
            if (squashedRepeatedList)
            {
                throw new NotImplementedException();
            }
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
        }

        public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
        {
            throw new NotImplementedException();
        }
    }
}
