using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Generation;
using Mutagen.Bethesda.Binary;

namespace Mutagen.Bethesda.Generation
{
    public class ModKeyBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<ModKey>
    {
        public ModKeyBinaryTranslationGeneration()
            : base(expectedLen: null, typeName: null, nullable: null)
        {
            PreferDirectTranslation = false;
        }

        protected override string GenerateForTypicalWrapper(
            ObjectGeneration objGen, 
            TypeGeneration typeGen,
            Accessor dataAccessor)
        {
            return $"ModKey.Factory(BinaryStringUtility.ToZString({dataAccessor}))";
        }
    }
}
