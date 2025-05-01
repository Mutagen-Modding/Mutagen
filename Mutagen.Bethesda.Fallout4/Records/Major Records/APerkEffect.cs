using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Fallout4;

public partial class APerkEffect
{
    [Flags]
    public enum Flag
    {
        RunImmediately = 0x01,
        ReplaceDefault = 0x02,
    }
}

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

partial class APerkEffectBinaryOverlay
{
    public partial ParseResult FunctionParametersCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        return lastParsed;
    }
}