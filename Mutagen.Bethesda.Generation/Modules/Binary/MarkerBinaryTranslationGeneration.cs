using Loqui.Generation;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class MarkerBinaryTranslationGeneration : BinaryTranslationGeneration
{
    public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
    {
        return 0;
    }

    public override bool ShouldGenerateCopyIn(TypeGeneration typeGen) => true;
    public override bool ShouldGenerateWrite(TypeGeneration typeGen) => true;

    public override async Task GenerateCopyIn(StructuredStringBuilder sb, ObjectGeneration objGen, TypeGeneration typeGen, Accessor readerAccessor, Accessor itemAccessor, Accessor errorMaskAccessor, Accessor translationAccessor)
    {
        sb.AppendLine($"{readerAccessor}.ReadSubrecord();");
    }

    public override async Task GenerateCopyInRet(StructuredStringBuilder sb, ObjectGeneration objGen, TypeGeneration targetGen, TypeGeneration typeGen, Accessor readerAccessor, AsyncMode asyncMode, Accessor retAccessor, Accessor outItemAccessor, Accessor errorMaskAccessor, Accessor translationAccessor, Accessor converterAccessor, bool inline)
    {
        throw new NotImplementedException();
    }

    public override async Task GenerateWrite(StructuredStringBuilder sb, ObjectGeneration objGen, TypeGeneration typeGen, Accessor writerAccessor, Accessor itemAccessor, Accessor errorMaskAccessor, Accessor translationAccessor, Accessor converterAccessor)
    {
        sb.AppendLine($"using (HeaderExport.Subrecord(writer, RecordTypes.{typeGen.GetFieldData().RecordType.Value})) {{ }}");
    }

    public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
    {
        throw new NotImplementedException();
    }

    public override async Task GenerateWrapperRecordTypeParse(StructuredStringBuilder sb, ObjectGeneration objGen, TypeGeneration typeGen, Accessor locationAccessor, Accessor packageAccessor, Accessor converterAccessor)
    {
        sb.AppendLine($"stream.ReadSubrecord();");
    }
}