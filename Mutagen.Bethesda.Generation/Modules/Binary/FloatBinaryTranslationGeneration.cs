using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;

namespace Mutagen.Bethesda.Generation
{
    public class FloatBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<float>
    {
        public FloatBinaryTranslationGeneration() 
            : base(expectedLen: 4, typeName: "Float")
        {
            PreferDirectTranslation = false;
        }

        protected override string GenerateForTypicalWrapper(
            ObjectGeneration objGen, 
            TypeGeneration typeGen, 
            Accessor dataAccessor)
        {
            return $"SpanExt.GetFloat({dataAccessor})";
        }
    }
}
