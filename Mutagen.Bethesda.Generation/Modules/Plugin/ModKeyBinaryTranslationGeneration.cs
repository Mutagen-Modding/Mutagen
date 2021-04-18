using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Generation;
using Mutagen.Bethesda.Binary;
using Noggog;

namespace Mutagen.Bethesda.Generation.Plugin
{
    public class ModKeyBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<ModKey>
    {
        public ModKeyBinaryTranslationGeneration()
            : base(expectedLen: null, typeName: null, nullable: null)
        {
            PreferDirectTranslation = false;
        }

        public override string GenerateForTypicalWrapper(
            ObjectGeneration objGen, 
            TypeGeneration typeGen,
            Accessor dataAccessor,
            Accessor packageAccessor)
        {
            return $"{nameof(ModKey)}.{nameof(ModKey.FromNameAndExtension)}({nameof(BinaryStringUtility)}.{nameof(BinaryStringUtility.ProcessWholeToZString)}({dataAccessor}))";
        }
    }
}
