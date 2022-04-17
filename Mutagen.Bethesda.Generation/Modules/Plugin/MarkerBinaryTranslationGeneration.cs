using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Generation.Modules.Binary;
using System;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class MarkerBinaryTranslationGeneration : BinaryTranslationGeneration
{
    public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
    {
        return 0;
    }

    public override bool ShouldGenerateCopyIn(TypeGeneration typeGen) => true;
    public override bool ShouldGenerateWrite(TypeGeneration typeGen) => true;

    public override async Task GenerateCopyIn(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor readerAccessor, Accessor itemAccessor, Accessor errorMaskAccessor, Accessor translationAccessor)
    {
        fg.AppendLine($"{readerAccessor}.ReadSubrecordFrame();");
    }

    public override void GenerateCopyInRet(FileGeneration fg, ObjectGeneration objGen, TypeGeneration targetGen, TypeGeneration typeGen, Accessor readerAccessor, AsyncMode asyncMode, Accessor retAccessor, Accessor outItemAccessor, Accessor errorMaskAccessor, Accessor translationAccessor, Accessor converterAccessor, bool inline)
    {
        throw new NotImplementedException();
    }

    public override async Task GenerateWrite(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor writerAccessor, Accessor itemAccessor, Accessor errorMaskAccessor, Accessor translationAccessor, Accessor converterAccessor)
    {
        fg.AppendLine($"using (HeaderExport.Subrecord(writer, RecordTypes.{typeGen.GetFieldData().RecordType.Value})) {{ }}");
    }

    public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
    {
        throw new NotImplementedException();
    }

    public override async Task GenerateWrapperRecordTypeParse(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor locationAccessor, Accessor packageAccessor, Accessor converterAccessor)
    {
        fg.AppendLine($"stream.ReadSubrecordFrame();");
    }
}