using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public abstract class BinaryTranslationGeneration : TranslationGeneration
    {
        public TranslationModule<BinaryTranslationGeneration> Module;
        public string Namespace => Module.Namespace;
        public virtual bool AllowDirectWrite(
            ObjectGeneration objGen,
            TypeGeneration typeGen) => true;
        public virtual bool AllowDirectParse(
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            bool squashedRepeatedList) => true;

        public abstract void GenerateWrite(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            string writerAccessor,
            Accessor itemAccessor,
            string maskAccessor,
            string translationAccessor);

        public abstract string GetTranslatorInstance(TypeGeneration typeGen);

        public abstract void GenerateCopyIn(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            string readerAccessor,
            Accessor itemAccessor,
            string maskAccessor,
            string translationAccessor);

        public abstract void GenerateCopyInRet(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration targetGen,
            TypeGeneration typeGen,
            string readerAccessor,
            bool squashedRepeatedList,
            string retAccessor,
            Accessor outItemAccessor,
            string maskAccessor,
            string translationAccessor);
    }
}
