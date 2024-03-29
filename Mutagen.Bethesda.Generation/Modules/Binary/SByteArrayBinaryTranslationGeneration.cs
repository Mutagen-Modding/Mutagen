using Loqui.Generation;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class SByteBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<sbyte>
{
    public SByteBinaryTranslationGeneration()
        : base(expectedLen: 1)
    {
        CustomRead = (sb, o, t, reader, item) =>
        {
            sb.AppendLine($"{item} = {reader}.ReadInt8();");
            return true;
        };
    }

    public override string GenerateForTypicalWrapper(
        ObjectGeneration objGen, 
        TypeGeneration typeGen,
        Accessor dataAccessor,
        Accessor packageAccessor)
    {
        return $"(sbyte){dataAccessor}[0]";
    }
}