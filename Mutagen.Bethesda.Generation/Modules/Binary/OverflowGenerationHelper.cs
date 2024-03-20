using Loqui.Generation;
using Mutagen.Bethesda.Generation.Modules.Plugin;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class OverflowGenerationHelper
{
    public static void GenerateWrapperOverflowParse(StructuredStringBuilder sb, TypeGeneration typeGen,
        MutagenFieldData data)
    {
        if (data.OverflowRecordType.HasValue
            && data.BinaryOverlayFallback != BinaryGenerationType.Custom)
        {
            sb.AppendLine($"_{typeGen.Name}LengthOverride = lastParsed.{nameof(PreviousParse.LengthOverride)};");
            sb.AppendLine($"if (lastParsed.{nameof(PreviousParse.LengthOverride)}.HasValue)");
            using (sb.CurlyBrace())
            {
                sb.AppendLine($"stream.Position += lastParsed.{nameof(PreviousParse.LengthOverride)}.Value;");
            }
        }
    }

    public static void GenerateWrapperOverflowMember(StructuredStringBuilder sb, TypeGeneration typeGen)
    {
        sb.AppendLine($"private int? _{typeGen.Name}LengthOverride;");
    }
}