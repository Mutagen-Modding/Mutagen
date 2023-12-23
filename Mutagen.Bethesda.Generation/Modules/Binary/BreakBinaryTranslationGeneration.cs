using Loqui.Generation;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Generation.Modules.Plugin;
using Noggog;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class BreakBinaryTranslationGeneration : BinaryTranslationGeneration
{
    public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen) => 0;

    public override async Task GenerateCopyIn(StructuredStringBuilder sb, ObjectGeneration objGen, TypeGeneration typeGen, Accessor readerAccessor, Accessor itemAccessor, Accessor errorMaskAccessor, Accessor translationAccessor)
    {
        var breakType = typeGen as BreakType;
        if (breakType == null) return;
        sb.AppendLine($"if (dataFrame.Complete)");
        using (sb.CurlyBrace())
        {
            sb.AppendLine($"item.{VersioningModule.VersioningFieldName} |= {objGen.ObjectName}.{VersioningModule.VersioningEnumName}.Break{breakType.Index};");
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
            sb.AppendLine($"return TryGet<int?>.Succeed({enumName ?? "null"});");
        }
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
        throw new NotImplementedException();
    }

    public override async Task GenerateWrite(StructuredStringBuilder sb, ObjectGeneration objGen, TypeGeneration typeGen, Accessor writerAccessor, Accessor itemAccessor, Accessor errorMaskAccessor, Accessor translationAccessor, Accessor converterAccessor)
    {
    }

    public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
    {
        throw new NotImplementedException();
    }
}