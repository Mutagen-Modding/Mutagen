using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Generation;

namespace Mutagen.Bethesda.Generation
{
    public class BooleanBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<bool>
    {
        public BooleanBinaryTranslationGeneration() 
            : base(expectedLen: 1)
        {
        }

        public override string GenerateForTypicalWrapper(ObjectGeneration objGen, TypeGeneration typeGen, Accessor dataAccessor, Accessor packageAccessor)
        {
            return $"{dataAccessor}[0] == 1";
        }
    }
}
