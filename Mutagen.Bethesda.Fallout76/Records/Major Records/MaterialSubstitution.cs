using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Fallout76;

partial class MaterialSubstitutionBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryFNAMParsingCustom(
        MutagenFrame frame,
        IMaterialSubstitution item,
        PreviousParse lastParsed)
    {
        return null;
    }
}

partial class MaterialSubstitutionBinaryWriteTranslation
{
    public static partial void WriteBinaryFNAMParsingCustom(
        MutagenWriter writer,
        IMaterialSubstitutionGetter item)
    {
    }
}

partial class MaterialSubstitutionBinaryOverlay
{
    public partial ParseResult FNAMParsingCustomParse(
        OverlayStream stream,
        int offset,
        PreviousParse lastParsed)
    {
        return null;
    }
}
