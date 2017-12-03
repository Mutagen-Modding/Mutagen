using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;

namespace Mutagen.Bethesda.Generation
{
    public class DataBinaryTranslationModule : BinaryTranslationGeneration
    {
        public override bool ShouldGenerateCopyIn(TypeGeneration typeGen) => true;
        public override bool ShouldGenerateWrite(TypeGeneration typeGen) => true;

        public override void GenerateCopyIn(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, string readerAccessor, Accessor itemAccessor, string doMaskAccessor, string maskAccessor)
        {
        }

        public override void GenerateCopyInRet(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, string readerAccessor, string retAccessor, string doMaskAccessor, string maskAccessor)
        {
        }

        public override void GenerateWrite(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, string writerAccessor, Accessor itemAccessor, string doMaskAccessor, string maskAccessor)
        {
        }
    }
}
