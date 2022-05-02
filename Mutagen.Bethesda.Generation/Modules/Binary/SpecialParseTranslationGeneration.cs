using Loqui.Generation;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class SpecialParseTranslationGeneration : BinaryTranslationGeneration
{
    public override bool ShouldGenerateCopyIn(TypeGeneration typeGen) => true;
    public override bool ShouldGenerateWrite(TypeGeneration typeGen) => true;

    public override async Task GenerateCopyIn(
        StructuredStringBuilder sb, 
        ObjectGeneration objGen, 
        TypeGeneration typeGen,
        Accessor readerAccessor,
        Accessor itemAccessor,
        Accessor errorMaskAccessor,
        Accessor translationMaskAccessor)
    {
        var data = typeGen.GetFieldData();
        using (var args = sb.Call(
                   $"SpecialParse_{typeGen.Name}"))
        {
            args.AddPassArg("item");
            args.AddPassArg("frame");
        }
    }

    public override void GenerateCopyInRet(
        StructuredStringBuilder sb,
        ObjectGeneration objGen, 
        TypeGeneration typeGen, 
        TypeGeneration targetGen,
        Accessor readerAccessor,
        AsyncMode asyncMode,
        Accessor retAccessor,
        Accessor outItemAccessor,
        Accessor errorMaskAccessor,
        Accessor translationAccessor,
        Accessor converterAccessor,
        bool inline)
    {
        throw new NotImplementedException();
    }

    public override async Task GenerateWrite(
        StructuredStringBuilder sb, 
        ObjectGeneration objGen, 
        TypeGeneration typeGen,
        Accessor writerAccessor,
        Accessor itemAccessor,
        Accessor errorMaskAccessor,
        Accessor translationMaskAccessor,
        Accessor converterAccessor)
    {
        var data = typeGen.GetFieldData();
        using (var args = sb.Call(
                   $"{objGen.CommonClass(LoquiInterfaceType.ISetter, CommonGenerics.Class)}.SpecialWrite_{typeGen.Name}_Internal"))
        {
            args.AddPassArg("item");
            args.AddPassArg("writer");
        }
    }

    public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
    {
        throw new NotImplementedException();
    }

    public override async Task<int?> GetPassedAmount(ObjectGeneration objGen, TypeGeneration typeGen) => 0;

    public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen) => null;

    public override async Task GenerateWrapperRecordTypeParse(
        StructuredStringBuilder sb, 
        ObjectGeneration objGen, 
        TypeGeneration typeGen, 
        Accessor locationAccessor, 
        Accessor packageAccessor, 
        Accessor converterAccessor)
    {
        using (var args = sb.Call(
                   $"{typeGen.Name}SpecialParse"))
        {
            args.AddPassArg("stream");
            args.AddPassArg("offset");
            args.AddPassArg("type");
            args.AddPassArg("lastParsed");
        }
    }
}