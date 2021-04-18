using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Generation;
using Mutagen.Bethesda.Binary;
using Noggog;

namespace Mutagen.Bethesda.Generation.Plugin
{
    public class RecordTypeBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<RecordType>
    {
        public RecordTypeBinaryTranslationGeneration()
            : base(expectedLen: 4, typeName: null, nullable: null)
        {
            PreferDirectTranslation = false;
        }

        public override string GenerateForTypicalWrapper(
            ObjectGeneration objGen, 
            TypeGeneration typeGen,
            Accessor dataAccessor,
            Accessor packageAccessor)
        {
            return $"new RecordType({nameof(BinaryPrimitives)}.{nameof(BinaryPrimitives.ReadInt32LittleEndian)}({dataAccessor}))";
        }
    }
}
