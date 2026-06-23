using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Fallout3;

partial class APerkEffectBinaryOverlay
{
    public byte Rank => throw new NotImplementedException();

    public byte Priority => throw new NotImplementedException();

    public IReadOnlyList<IPerkConditionGetter> Conditions => throw new NotImplementedException();
}

partial class APerkEffectBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryFunctionParametersCustom(MutagenFrame frame, IAPerkEffect item, PreviousParse lastParsed)
    {
        return lastParsed;
    }
}

partial class APerkEffectBinaryWriteTranslation
{
    public static partial void WriteBinaryFunctionParametersCustom(MutagenWriter writer, IAPerkEffectGetter item)
    {
    }
}
