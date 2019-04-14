using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;
using Noggog;

namespace Mutagen.Bethesda.Generation
{
    public class FormKeyBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<FormKey>
    {
        public FormKeyBinaryTranslationGeneration()
        {
            this.AdditionalWriteParams.Add(AdditionalParam);
            this.AdditionalCopyInParams.Add(AdditionalParam);
            this.AdditionalCopyInRetParams.Add(AdditionalParam);
        }

        private static TryGet<string> AdditionalParam(
           ObjectGeneration objGen,
           TypeGeneration typeGen)
        {
            return TryGet<string>.Succeed("masterReferences: masterReferences");
        }
    }
}
