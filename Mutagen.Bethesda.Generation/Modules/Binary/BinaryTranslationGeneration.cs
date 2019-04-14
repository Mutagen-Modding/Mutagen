using Loqui;
using Loqui.Generation;
using Noggog;
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

        public delegate TryGet<string> ParamTest(
            ObjectGeneration objGen,
            TypeGeneration typeGen);
        public List<ParamTest> AdditionalWriteParams = new List<ParamTest>();
        public List<ParamTest> AdditionalCopyInParams = new List<ParamTest>();
        public List<ParamTest> AdditionalCopyInRetParams = new List<ParamTest>();

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
            Accessor writerAccessor,
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationAccessor);

        public abstract string GetTranslatorInstance(TypeGeneration typeGen);

        public abstract void GenerateCopyIn(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor readerAccessor,
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationAccessor);

        public abstract void GenerateCopyInRet(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration targetGen,
            TypeGeneration typeGen,
            Accessor readerAccessor,
            bool squashedRepeatedList,
            Accessor retAccessor,
            Accessor outItemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationAccessor);
    }
}
