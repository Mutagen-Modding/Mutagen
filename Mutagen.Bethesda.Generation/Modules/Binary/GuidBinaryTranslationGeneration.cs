using Loqui.Generation;
using Mutagen.Bethesda.Generation.Modules.Binary;

public class GuidBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<Guid>
{
    public GuidBinaryTranslationGeneration() 
        : base(expectedLen: 16)
    {
        PreferDirectTranslation = false;
    }

    public override bool NeedsGenerics => false;
    
    public override string GenerateForTypicalWrapper(
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor dataAccessor,
        Accessor packageAccessor)
    {
        return $"new Guid({dataAccessor}.Slice(0, 16))";
    }
}