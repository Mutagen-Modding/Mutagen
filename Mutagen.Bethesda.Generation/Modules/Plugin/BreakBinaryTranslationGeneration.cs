using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Generation.Modules.Binary;
using Noggog;
using Mutagen.Bethesda.Generation.Fields;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class BreakBinaryTranslationGeneration : BinaryTranslationGeneration
{
    public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen) => 0;

    public override async Task GenerateCopyIn(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor readerAccessor, Accessor itemAccessor, Accessor errorMaskAccessor, Accessor translationAccessor)
    {
        var breakType = typeGen as BreakType;
        if (breakType == null) return;
        fg.AppendLine($"if (dataFrame.Complete)");
        using (new BraceWrapper(fg))
        {
            fg.AppendLine($"item.{VersioningModule.VersioningFieldName} |= {objGen.ObjectName}.{VersioningModule.VersioningEnumName}.Break{breakType.Index};");
            string enumName = null;
            var startIndex = objGen.Fields.IndexOf(typeGen);
            for (int i = startIndex - 1; i >= 0; i--)
            {
                if (!objGen.Fields.TryGet(i, out var prevField)) continue;
                if (!prevField.IntegrateField) continue;
                enumName = prevField.IndexEnumName;
                break;
            }
            if (enumName != null)
            {
                enumName = $"(int){enumName}";
            }
            fg.AppendLine($"return TryGet<int?>.Succeed({enumName ?? "null"});");
        }
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
        throw new NotImplementedException();
    }

    public override async Task GenerateWrite(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor writerAccessor, Accessor itemAccessor, Accessor errorMaskAccessor, Accessor translationAccessor, Accessor converterAccessor)
    {
    }

    public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
    {
        throw new NotImplementedException();
    }
}