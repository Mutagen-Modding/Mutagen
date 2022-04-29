using Loqui;
using Loqui.Generation;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class NothingBinaryTranslationGeneration : BinaryTranslationGeneration
{
    public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
    {
        return 0;
    }

    public override async Task GenerateCopyIn(
        StructuredStringBuilder sb, 
        ObjectGeneration objGen, 
        TypeGeneration typeGen,
        Accessor readerAccessor, 
        Accessor itemAccessor, 
        Accessor errorMaskAccessor,
        Accessor translationAccessor)
    {
    }

    public override void GenerateCopyInRet(
        StructuredStringBuilder sb, 
        ObjectGeneration objGen, 
        TypeGeneration targetGen, 
        TypeGeneration typeGen, 
        Accessor readerAccessor, 
        AsyncMode asyncMode, 
        Accessor retAccessor, 
        Accessor outItemAccessor, 
        Accessor errorMaskAccessor, 
        Accessor translationAccessor,
        Accessor converterAccessor,
        bool inline)
    {
    }

    public override async Task GenerateWrite(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration typeGen, 
        Accessor writerAccessor, 
        Accessor itemAccessor, 
        Accessor errorMaskAccessor, 
        Accessor translationAccessor,
        Accessor converterAccessor)
    {
    }

    public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
    {
        throw new NotImplementedException();
    }
}