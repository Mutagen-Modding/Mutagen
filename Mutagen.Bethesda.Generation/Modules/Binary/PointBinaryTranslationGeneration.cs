using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Generation;

namespace Mutagen.Bethesda.Generation
{
    public class PointBinaryTranslationGeneration<T> : PrimitiveBinaryTranslationGeneration<T>
    {
        public PointBinaryTranslationGeneration(int? expectedLen)
            : base(expectedLen)
        {
            PreferDirectTranslation = false;
        }

        public override string GenerateForTypicalWrapper(ObjectGeneration objGen, TypeGeneration typeGen, Accessor dataAccessor, Accessor packageAccessor)
        {
            return $"{this.Typename(typeGen)}BinaryTranslation.Read({dataAccessor})";
        }
    }
}
