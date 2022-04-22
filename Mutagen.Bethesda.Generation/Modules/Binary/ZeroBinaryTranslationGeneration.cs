using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Generation.Fields;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class ZeroBinaryTranslationGeneration : BinaryTranslationGeneration
{
    public override bool ShouldGenerateCopyIn(TypeGeneration typeGen)
    {
        return true;
    }

    public override async Task GenerateCopyIn(
        FileGeneration fg,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor readerAccessor,
        Accessor itemAccessor,
        Accessor errorMaskAccessor,
        Accessor translationMaskAccessor)
    {
        ZeroType zero = typeGen as ZeroType;
        fg.AppendLine($"{readerAccessor}.SetPosition({readerAccessor}.Position + {zero.Length});");
    }

    public override void GenerateCopyInRet(
        FileGeneration fg, 
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
        if (inline) throw new NotImplementedException();
        if (asyncMode == AsyncMode.Direct) throw new NotImplementedException();
        ZeroType zero = typeGen as ZeroType;
        fg.AppendLine($"{readerAccessor}.SetPosition({readerAccessor}.Position + {zero.Length});");
    }

    public override async Task GenerateWrite(
        FileGeneration fg,
        ObjectGeneration objGen, 
        TypeGeneration typeGen,
        Accessor writerAccessor, 
        Accessor itemAccessor,
        Accessor errorMaskAccessor,
        Accessor translationMaskAccessor,
        Accessor converterAccessor)
    {
        ZeroType zero = typeGen as ZeroType;
        fg.AppendLine($"{writerAccessor}.WriteZeros({zero.Length});");
    }

    public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
    {
        throw new NotImplementedException();
    }

    public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
    {
        ZeroType zero = typeGen as ZeroType;
        return zero.Length;
    }
}