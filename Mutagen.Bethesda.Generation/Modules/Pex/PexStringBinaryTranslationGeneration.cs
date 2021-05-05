using Loqui.Generation;
using Mutagen.Bethesda.Generation.Modules.Binary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation.Modules.Pex
{
    public class PexStringBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<string>
    {
        public override bool NeedsGenerics => false;

        public PexStringBinaryTranslationGeneration(string typeName = null)
            : base(expectedLen: 2, typeName, nullable: false)
        {
        }
    }
}
