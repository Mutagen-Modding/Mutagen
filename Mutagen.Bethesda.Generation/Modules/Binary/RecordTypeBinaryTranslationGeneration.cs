using System.Buffers.Binary;
using Loqui.Generation;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class RecordTypeBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<RecordType>
{
    public override bool NeedsGenerics => false;

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