using Loqui.Generation;
using Mutagen.Bethesda.Generation.Modules.Binary;

public class CharBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<char>
{
    public CharBinaryTranslationGeneration() 
        : base(expectedLen: 1)
    {
    }

    public override string GenerateForTypicalWrapper(
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor dataAccessor,
        Accessor packageAccessor)
    {
        return $"(char){dataAccessor}[0]";
    }
}