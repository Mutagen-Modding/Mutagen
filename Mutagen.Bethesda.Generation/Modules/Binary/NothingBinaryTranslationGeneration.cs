using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Generation
{
    public class NothingBinaryTranslationGeneration : BinaryTranslationGeneration
    {
        public override int? ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
        {
            return 0;
        }

        public override void GenerateCopyIn(
            FileGeneration fg, 
            ObjectGeneration objGen, 
            TypeGeneration typeGen,
            Accessor readerAccessor, 
            Accessor itemAccessor, 
            Accessor errorMaskAccessor,
            Accessor translationAccessor)
        {
        }

        public override void GenerateCopyInRet(
            FileGeneration fg, 
            ObjectGeneration objGen, 
            TypeGeneration targetGen, 
            TypeGeneration typeGen, 
            Accessor readerAccessor, 
            AsyncMode asyncMode, 
            Accessor retAccessor, 
            Accessor outItemAccessor, 
            Accessor errorMaskAccessor, 
            Accessor translationAccessor,
            Accessor converterAccessor)
        {
        }

        public override void GenerateWrite(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen, 
            Accessor writerAccessor, 
            Accessor itemAccessor, 
            Accessor errorMaskAccessor, 
            Accessor translationAccessor)
        {
        }

        public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
        {
            throw new NotImplementedException();
        }
    }
}
