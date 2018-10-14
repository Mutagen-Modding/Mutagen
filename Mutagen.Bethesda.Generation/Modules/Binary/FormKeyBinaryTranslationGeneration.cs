using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;

namespace Mutagen.Bethesda.Generation
{
    public class FormKeyBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<FormKey>
    {
        protected override IEnumerable<string> AdditionalWriteParameters(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, string writerAccessor, Accessor itemAccessor, string maskAccessor)
        {
            yield return "masterReferences: masterReferences";
        }

        protected override IEnumerable<string> AdditionalCopyInParameters(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, string nodeAccessor, Accessor itemAccessor, string maskAccessor)
        {
            yield return "masterReferences: masterReferences";
        }

        protected override IEnumerable<string> AdditionalCopyInRetParameters(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, string nodeAccessor, string retAccessor, Accessor outItemAccessor, string maskAccessor)
        {
            yield return "masterReferences: masterReferences";
        }
    }
}
