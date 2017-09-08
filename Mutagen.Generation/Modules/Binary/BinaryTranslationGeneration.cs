using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Generation
{
    public abstract class BinaryTranslationGeneration
    {
        public MaskModule MaskModule;
        public TranslationModule<BinaryTranslationGeneration> Module;
        public string Namespace => Module.Namespace;

        public abstract void GenerateWrite(
            FileGeneration fg,
            TypeGeneration typeGen,
            string writerAccessor,
            string itemAccessor,
            string doMaskAccessor,
            string maskAccessor);

        public virtual bool ShouldGenerateCopyIn(TypeGeneration typeGen) => true;

        public abstract void GenerateCopyIn(
            FileGeneration fg,
            TypeGeneration typeGen,
            string readerAccessor,
            Accessor itemAccessor,
            string doMaskAccessor,
            string maskAccessor);

        public abstract void GenerateCopyInRet(
            FileGeneration fg,
            TypeGeneration typeGen,
            string readerAccessor,
            string retAccessor,
            string doMaskAccessor,
            string maskAccessor);
    }
}
